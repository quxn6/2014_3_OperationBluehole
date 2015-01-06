using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Matching.Worker
{
	using System.Threading;
	using System.Diagnostics;
    using System.Collections.Concurrent;
	using LitJson;
	using OperationBluehole.Content;
    using OperationBluehole.Database;
    using RabbitMQ.Client;

	using RegData = Tuple<OperationBluehole.Content.PlayerData, int, List<string>, System.Diagnostics.Stopwatch>; // Tuple<플레이어, 추가난이도, 차단리스트, 등록 시간>

    class MatchingData
    {
		public Stopwatch regTime;
        public ushort level;
        public int difficulty;
		public List<RegData> members;
    }

	class Matching
	{
		private readonly RabbitMQ.Client.ConnectionFactory _connectionFactory;

		public ushort minLev;
		public ushort maxLev;

		public BlockingCollection<RegData> waitingPlayers { get; private set; } // 대기 플레이어
		List<string> deregisterWaitingPlayers; // 등록 해제 요청 목록
		List<RegData> inPartyPlayers; // 파티에 속한 플레이어
		public List<MatchingData> waitingParties { get; private set; }

		public Matching( ushort minLev, ushort maxLev, string simulationAddr )
		{
			_connectionFactory = new ConnectionFactory() { HostName = simulationAddr };

			this.minLev = minLev;
			Program.form.UpdateMinLev( this.minLev );
			this.maxLev = maxLev;
			Program.form.UpdateMaxLev( this.maxLev );

			waitingPlayers = new BlockingCollection<RegData>();
			deregisterWaitingPlayers = new List<string>();
			inPartyPlayers = new List<RegData>();
			waitingParties = new List<MatchingData>();

			Task.Factory.StartNew( MatchPlayer );
		}

		public bool RegisterPlayer( string playerId, int difficulty )
		{
			var playerData = PlayerDataDatabase.GetPlayerData( playerId );
			Debug.Assert( playerData != null, "player data is null : " + playerId );

			var userData = UserDataDatabase.GetUserData( playerId );
			Debug.Assert( userData != null, "user data is null : " + playerId );

			return RegisterPlayer( new RegData( playerData, difficulty, userData.BanList, Stopwatch.StartNew() ) );
		}
		public bool RegisterPlayer( RegData data )
		{
			// 이미 대기 목록에 있다면 false리턴
			var res = waitingPlayers.FirstOrDefault( i => i.Item1 == data.Item1 );
			if ( res != null )
			{
				Debug.WriteLine( "player is already registered : " + data.Item1.pId );
				return false;
			}

			waitingPlayers.Add( data );
			Program.form.UpdateWaitingPlayers( waitingPlayers.Count );

			Debug.WriteLine( "registered player id : " + data.Item1.pId );
			return true;
		}

		public void DeregisterPlayer( string playerId )
		{
			deregisterWaitingPlayers.Add( playerId );
		}

		void RegisterParty( RegData firstRegData )
		{
			MatchingData newMd = new MatchingData();
			newMd.members = new List<RegData>();
			newMd.members.Add( firstRegData );
			newMd.level = firstRegData.Item1.stats[(int)StatType.Lev];
			newMd.difficulty = firstRegData.Item2;
			newMd.regTime = Stopwatch.StartNew();

			waitingParties.Add( newMd );
			Program.form.UpdateWaitingParties( waitingParties.Count );
		}

		void DeregisterParty( MatchingData md )
		{
			waitingParties.Remove( md );
			Program.form.UpdateWaitingParties( waitingParties.Count );

			// 파티에 있는 유저 모두 inPartyPlayers와 waitingParties에서 제거
			md.members.ForEach( mem => {
				inPartyPlayers.Remove( mem );
				Program.form.UpdateInPartyPlayers( inPartyPlayers.Count );

				waitingParties.ForEach( tMd =>
				{
					tMd.members.Remove( mem );

					// 유저를 제거후 파티가 비었다면 파티 삭제
					if ( tMd.members.Count == 0 )
					{
						waitingParties.Remove( md );
						Program.form.UpdateWaitingParties( waitingParties.Count );
					}
				});
			});
		}

		public void Reset()
		{
			waitingPlayers.Add( new RegData( null, Config.MATCHING_THREAD_END_DIFFCULTY, null, null ) );
			Program.form.UpdateWaitingPlayers( waitingPlayers.Count );

			while ( waitingPlayers.Count > 0 )
				Thread.Sleep( 10 );

			waitingParties.Clear();
			Program.form.UpdateWaitingParties( waitingParties.Count );

			var tmp = inPartyPlayers;
			inPartyPlayers = new List<RegData>();
			Program.form.UpdateInPartyPlayers( inPartyPlayers.Count );

			Task.Factory.StartNew( MatchPlayer );
			tmp.ForEach( data => RegisterPlayer( data ) );
		}

		public void MatchPlayer()
		{
			foreach ( var data in waitingPlayers.GetConsumingEnumerable() )
			{
				if ( data.Item2 == Config.MATCHING_THREAD_END_DIFFCULTY ) // 강제 종료
					break;

				// 등록 해제 목록에 있을시 해당 유저 처리하지 않음
				if ( deregisterWaitingPlayers.Remove( data.Item1.pId ) )
					continue;

				// 등록 해제 목록에 있는 유저 모두 waitingParties에서 제거
				waitingParties.ForEach( md =>
				{
					md.members.RemoveAll( m =>
						deregisterWaitingPlayers.Contains( m.Item1.pId )
					);

					// 유저를 제거후 파티가 비었다면 파티 삭제
					if ( md.members.Count == 0 )
					{
						waitingParties.Remove( md );
						Program.form.UpdateWaitingParties( waitingParties.Count );
					}
				} );
				deregisterWaitingPlayers.Clear();

				// 레벨 구간에 맞지 않으면 되돌려보냄
				if ( data.Item1.stats[(int)StatType.Lev] < minLev || data.Item1.stats[(int)StatType.Lev] > maxLev )
				{
					var newDic = new Dictionary<string, object>();
					newDic.Add( "playerId", data.Item1.pId );
					newDic.Add( "difficulty", data.Item2 );
					TaskManager.SendToLobby( "MATCH", newDic );

					if ( maxLev < minLev && waitingPlayers.Count == 0 ) // 더 이상 사용하지 않는 스레드이므로 종료
						break;
					continue;
				}

				inPartyPlayers.Add( data );
				Program.form.UpdateInPartyPlayers( inPartyPlayers.Count );

				// 조건에 맞는 파티 검색
				var resList = waitingParties
					.Where( md =>
						Math.Abs( md.level - data.Item1.stats[(int)StatType.Lev] ) <= Config.MATCHING_ALLOW_LEVEL_DIFF
						&& md.difficulty == data.Item2
						&& !md.members.Exists( p =>
							p.Item3.Contains( data.Item1.pId )
							&& data.Item3.Contains( p.Item1.pId )
						)
					);

				// 조건에 맞는 파티가 없으면 새로 만듬
				if ( resList.Count() == 0 )
				{
					RegisterParty( data );
					continue;
				}

				// 조건에 맞는 파티에 전부 집어 넣어본다
				foreach ( var md in resList )
				{
					if ( md.members.Count < Config.MATCHING_PARTY_MEMBERS_NUM )
						md.members.Add( data );

					// 인원이 꽉차 매칭된 파티가 생기면 매칭된 멤버들은 스레드에서 제거
					if ( md.members.Count == Config.MATCHING_PARTY_MEMBERS_NUM )
					{
						DeregisterParty( md );
						MatchParty( md );
						break;
					}
				}
			}
		}

		void MatchParty( MatchingData md )
		{
			Int64 curTime = Stopwatch.GetTimestamp();
			LogRecord.Write( "[Party Matching Time : " + md.regTime.ElapsedMilliseconds + " ms]" );
			foreach ( var member in md.members )
			{
				LogRecord.Write( "[Player : " + member.Item1.pId + "] [Matching Time : " + member.Item4.ElapsedMilliseconds + " ms]" );
			}

			int partyLevel = (int)md.members.Average( each => each.Item1.stats[0] );
			partyLevel += md.difficulty;
			if ( partyLevel < 1 )
				partyLevel = 1;

			using ( var connection = _connectionFactory.CreateConnection() )
			{
				using ( var channel = connection.CreateModel() )
				{
					channel.QueueDeclare( "simulation_queue", true, false, false, null );

					var newDic = new Dictionary<string, object>();
					newDic.Add( "level", partyLevel );
					for ( int idx = 0; idx < Config.MATCHING_PARTY_MEMBERS_NUM; ++idx )
					{
						newDic.Add( "char_" + idx, md.members[idx].Item1.pId );
					}

					var message = JsonMapper.ToJson( newDic );
					var body = Encoding.UTF8.GetBytes( message );

					var properties = channel.CreateBasicProperties();
					properties.SetPersistent( true );

					channel.BasicPublish( "", "simulation_queue", properties, body );
					Console.WriteLine( " [x] Sent {0}", message );
				}
			}
		}
	}
}
