using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using OperationBluehole.Content;

namespace OperationBluehole.DummyClient
{
    // using Newtonsoft.Json;
    using LitJson;

    internal class ClientPlayerData
    {
        public string Name { get; set; }

        public uint Exp { get; set; }
        public ushort StatPoints { get; set; }
        public List<ushort> Stat { get; set; }
        public List<ushort> Skill { get; set; }

        public uint Gold { get; set; }
        public List<uint> Inventory { get; set; }
        public List<ItemToken> Token { get; set; }

        public List<uint> Equipment { get; set; }
        public List<uint> Consumable { get; set; }

        public byte BattleStyle { get; set; }

        public List<string> BanList { get; set; }

        public ClientPlayerData()
        {
        }
    }

    internal class SimulationResult
    {
        public long Id { get; set; }

        // 참가한 player id 목록
        public List<PlayerData> PlayerList { get; set; }

        // 맵 크기
        public int MapSize { get; set; }

        // 게임 결과를 확인한 플레이어 - 전부 확인하면 지울 수 있도록? 아니면 아예 무조건 타임아웃? 적절히 혼합?
        public List<ulong> CheckedPlayer { get; set; }

        // 시뮬레이션에 사용한 random seed 값
        public int Seed { get; set; }
    }

	class User
	{
        const uint MAX_SIMULATION_COUNT = 10;

		string userId, password;
		string token;
        ClientPlayerData playerData;
        uint simulationCount;
        Random random;

		public User(string userId, string password, Random random)
		{
			this.userId = userId;
            this.password = password;
            this.random = random;
            playerData = null;
            simulationCount = 0;
		}

		public async void Start()
		{
            // 가입
			{
				bool chk = await Network.SignIn(this.userId, this.password, this.userId + "_1");
				Console.WriteLine("SignIn : {0}", chk);
			}

            // 로그인
			{
				this.token = await Network.LogIn(userId, password);
				Console.WriteLine("Login Token : {0}", this.token);
			}

            // 로그인 확인
			{
				bool chk = await Network.IsLogIn(this.token);
				Console.WriteLine("Login Status : {0}", chk);
			}

            // 플레이어 데이터 받아오기
            {
                string data = await Network.GetPlayerData( this.token );

                if ( !string.IsNullOrEmpty(data) )
                {
                    // playerData = Json.JsonParser.Deserialize<ClientPlayerData>( data );
                    // playerData = JsonConvert.DeserializeObject<ClientPlayerData>( data );
                    playerData = JsonMapper.ToObject<ClientPlayerData>( data );
                
                    Console.WriteLine( "player data updated" );
                }
                else
                {
                    Console.WriteLine( "failed to get player data" );
                    return;
                }
            }

            Task.Delay( 500 ).GetAwaiter().OnCompleted( () => this.Register() );
		}

        private async void Register()
        {
            // 대기열 등록
            {
                string result;

                result = await Network.RegisterPlayer( this.token, 1 );   // difficulty 일단 1로 고정

                if ( result.CompareTo( "success" ) == 0 )
                {
                    Console.WriteLine( "player registered" );

                    Task.Delay( 2000 ).GetAwaiter().OnCompleted( () => this.UpdateResult() );
                }
                else
                {
                    Console.WriteLine( "there is unread game result" );

                    Task.Run( () => this.UpdateResult() );
                }
            }
        }

        private async void UpdateResult()
        {
            // 결과 확인
            {
                string result = await Network.GetSimulationResult( this.token );

                if ( result.CompareTo("nothing") == 0 )
                {
                    Console.WriteLine( "not yet" );
                    Task.Delay( 2000 ).GetAwaiter().OnCompleted( () => this.UpdateResult() );
                    return;
                }
                else if ( result.CompareTo( "error") == 0 )
                {
                    return;
                }
                else
                {
                    Console.WriteLine( "game result updated" );
                }
            }

            // 플레이어 데이터 받아오기
            {
                string data = await Network.GetPlayerData( this.token );

                if ( !string.IsNullOrEmpty( data ) )
                {
                    // playerData = Json.JsonParser.Deserialize<ClientPlayerData>( data );
                    // playerData = JsonConvert.DeserializeObject<ClientPlayerData>( data );
                    playerData = JsonMapper.ToObject<ClientPlayerData>( data );
                
                    Console.WriteLine( "player data updated" );
                }
                else
                {
                    Console.WriteLine( "failed to get player data" );
                    return;
                }
            }

            // 레벨업
            {
                string result = "";
                do
                {
                    result = await Network.LevelUp( this.token );
                } while ( result.CompareTo( "level up" ) == 0 );

                Console.WriteLine( "level up!" );
            }

            // 스탯 상승
            {
                int totalStats = ( playerData.Stat[(int)StatType.Lev] - 1 ) * 3 + 35;
                int curStats = playerData.Stat.Skip( 1 ).Take( 6 ).Sum( i => i );
                
                int bonusStat = totalStats - curStats;

                if ( bonusStat > 0 )
                {
                    ushort[] stats = new ushort[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    for ( int i = bonusStat; i > 0; --i )
                    {
                        ++stats[random.Next( (int)StatType.Str, (int)StatType.Mov )];
                    }

                    string result = await Network.IncreaseStats( this.token, stats );

                    if ( result.CompareTo( "failed" ) == 0 )
                        Console.WriteLine( "failed to increase player stats" );
                    else
                        Console.WriteLine( "increase stats" );
                }
            }

            Task.Delay( 500 ).GetAwaiter().OnCompleted( () => this.Register() );
        }
	}
}
