using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Matching
{
    using System.Threading;
    using System.Diagnostics;
    using System.Collections.Concurrent;

    using OperationBluehole.Content;
    using OperationBluehole.Database;
    using RabbitMQ.Client;
    using System.Text;
    using LitJson;

	using RegData = Tuple<OperationBluehole.Content.PlayerData, int, List<string>, Int64>; // Tuple<플레이어, 추가난이도, 차단리스트, 등록 시간>

    struct MatchingData
    {
        public Int64 regTime;
        public ushort level;
        public int difficulty;
		public List<RegData> members;
    }

    public static class MatchingManager
    {
		static SynchronizedCollection<MatchingThread> matchingThreads;
        private readonly static RabbitMQ.Client.ConnectionFactory _connectionFactory;

        static MatchingManager()
        {
            _connectionFactory = new ConnectionFactory() { HostName = Config.SIMULATION_QUEUE_ADDRESS };

			matchingThreads = new SynchronizedCollection<MatchingThread>();
        }

		public static void Init()
		{
			NewMatchingThread( 1, ushort.MaxValue );

			Debug.WriteLine( "matching manager is started" );
		}

		public static void RegisterPlayer( string playerId, int difficulty )
        {
            var playerData = PlayerDataDatabase.GetPlayerData( playerId );
            Debug.Assert( playerData != null, "player data is null : " + playerId );

            var userData = UserDataDatabase.GetUserData( playerId );
            Debug.Assert( userData != null, "user data is null : " + playerId );

			RegisterPlayer( new RegData( playerData, difficulty, userData.BanList, Stopwatch.GetTimestamp() ) );
        }
		public static void RegisterPlayer( RegData data )
		{
			var lev = data.Item1.stats[(int)StatType.Lev];
			matchingThreads.First( mt => mt.maxLev >= lev && mt.minLev <= lev ).RegisterPlayer( data ); // 좀 빠르게 고쳐야...
		}

        public static void DeregisterPlayer( string playerId )
        {
			var playerData = PlayerDataDatabase.GetPlayerData( playerId );
			DeregisterPlayer( playerData );
        }
		public static void DeregisterPlayer( PlayerData playerData )
		{
			var lev = playerData.stats[(int)StatType.Lev];
			matchingThreads.First( mt => mt.maxLev >= lev && mt.minLev <= lev ).DeregisterPlayer( playerData.pId ); // 좀 빠르게 고쳐야...
		}

        // 해당 레벨 구간을 매칭하는 스레드를 만든다.
		public static void NewMatchingThread( ushort minLev, ushort maxLev )
		{
			Debug.Assert( minLev > 0 && maxLev >= minLev );

			// 해당 구간 스레드 생성
			if ( matchingThreads.FirstOrDefault( mt => mt.minLev == minLev && mt.maxLev == maxLev ) == null )
				matchingThreads.Add( new MatchingThread( minLev, maxLev ) );
			else
				return;

			// 해당 구간에 포함되는 스레드 삭제
			//      [Thread]
			// [new Thread        ]
			{
				var res = matchingThreads.Where( mt =>
					minLev < mt.minLev
					&& mt.maxLev < maxLev
					);
				foreach ( var mt in res )
				{
					matchingThreads.Remove( mt );
					mt.maxLev = 0;
				}
			}

			// 해당 구간과 겹치는 스레드 조정
			// [Thread            ]
			//     [new Thread]
			{
				var res = matchingThreads.Where( mt =>
					mt.minLev < minLev
					&& maxLev < mt.maxLev
					).FirstOrDefault();
				if ( res != null )
				{
					matchingThreads.Add( new MatchingThread( (ushort)(maxLev + 1), res.maxLev ) );
					res.maxLev = (ushort)(minLev - 1);
				}
			}

			// 해당 구간과 겹치는 스레드 조정
			// [Thread     ]
			//        [new Thread]
			{
				var res = matchingThreads.Where( mt =>
					mt.minLev < minLev
					&& minLev <= mt.maxLev
					).FirstOrDefault();
				if ( res != null )
					res.maxLev = (ushort)(minLev - 1);
			}

			// 해당 구간과 겹치는 스레드 조정
			//       [Thread     ]
			// [new Thread]
			{
				var res = matchingThreads.Where( mt =>
					mt.minLev <= maxLev
					&& maxLev < mt.maxLev
					).FirstOrDefault();
				if ( res != null )
					res.minLev = (ushort)(maxLev + 1);
			}
		}





		class MatchingThread
		{
			public ushort minLev;
			public ushort maxLev;

			public BlockingCollection<RegData> waitingPlayers { get; private set; }
			List<string> deregisterWaitingPlayers; // 등록 해제 요청 목록
			List<MatchingData> waitingParties;

			public MatchingThread( ushort minLev, ushort maxLev )
			{
				this.minLev = minLev;
				this.maxLev = maxLev;

				waitingPlayers = new BlockingCollection<RegData>();
				deregisterWaitingPlayers = new List<string>();
				waitingParties = new List<MatchingData>();

				Task.Factory.StartNew( MatchPlayer );
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
				Debug.WriteLine( "registered player id : " + data.Item1.pId );
				return true;
			}

			public void DeregisterPlayer( string playerId )
			{
				deregisterWaitingPlayers.Add( playerId );
			}

			void MatchPlayer()
			{
				foreach ( var data in waitingPlayers.GetConsumingEnumerable() )
				{
					if ( data.Item2 == Config.MATCHING_THREAD_END_DIFFCULTY ) // 강제 종료
						break;

					// 등록 해제 목록에 있을시 해당 유저 처리하지 않음
					if ( deregisterWaitingPlayers.Remove( data.Item1.pId ) )
						continue;

					// 레벨 구간에 맞지 않으면 되돌려보냄
					if ( data.Item1.stats[(int)StatType.Lev] < minLev || data.Item1.stats[(int)StatType.Lev] > maxLev )
					{
						MatchingManager.RegisterPlayer( data );
						if ( maxLev < minLev && waitingPlayers.Count == 0 ) // 더 이상 사용하지 않는 스레드이므로 종료
							break;
						continue;
					}

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
						MatchingData newMd = new MatchingData();
						newMd.members = new List<RegData>();
						newMd.members.Add( data );
						newMd.level = data.Item1.stats[(int)StatType.Lev];
						newMd.difficulty = data.Item2;
						newMd.regTime = Stopwatch.GetTimestamp();

						waitingParties.Add( newMd );
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
							waitingParties.Remove( md );
							MatchParty( md );
							foreach ( var member in md.members )
								DeregisterPlayer( member.Item1.pId );
							break;
						}
					}

					// waitingParties에서 등록 해제 목록에 있는 유저 모두 제거
					waitingParties.ForEach( md =>
					{
						md.members.RemoveAll( m =>
							deregisterWaitingPlayers.Contains( m.Item1.pId )
						);

						// 해당 파티가 비었다면 파티 삭제
						if ( md.members.Count == 0 )
							waitingParties.Remove( md );
					} );
					deregisterWaitingPlayers.Clear();
				}
			}

			void MatchParty( MatchingData md )
			{
				Int64 curTime = Stopwatch.GetTimestamp();
				LogRecord.Write( "[Party Matching Time : " + (curTime - md.regTime) / LogRecord.tickPerMillisecond + " ms]" );
				foreach ( var member in md.members )
				{
					LogRecord.Write( "[Player : " + member.Item1.pId + "] [Matching Time : " + (curTime - member.Item4) / LogRecord.tickPerMillisecond + " ms]" );
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
}
