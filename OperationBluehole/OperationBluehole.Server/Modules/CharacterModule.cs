using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace OperationBluehole.Server.Modules
{
    using Nancy;
    using Nancy.Security;
    using Nancy.Authentication.Token;

    using Newtonsoft.Json;
    using OperationBluehole.Content;
    using OperationBluehole.Database;

    // 플레이어의 캐릭터를 조작( 장비 설정이나 능력치 배분과 같은... )

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

        public ClientPlayerData( PlayerData playerData, UserData userData )
        {
            // player data
            this.Name = playerData.name;

            this.Exp = playerData.exp;

            this.Stat = new List<ushort>();
            for ( int i = 0; i < (int)StatType.StatCount; ++i )
                this.Stat.Add( playerData.stats[i] );

            this.Skill = new List<ushort>();
            playerData.skills.ForEach( each => this.Skill.Add( (ushort)each ) );

            this.Equipment = new List<uint>();
            playerData.equipments.ForEach( each => this.Equipment.Add( (uint)each ) );

            this.Consumable = new List<uint>();
            playerData.consumables.ForEach( each => this.Consumable.Add( (uint)each ) );

            this.BattleStyle = (byte)playerData.battleStyle;

            // user data
            this.Gold = userData.Gold;

            this.Token = new List<ItemToken>();
            userData.Token.ForEach( each => this.Token.Add( each ) );

            this.Inventory = new List<uint>();
            userData.Inventory.ForEach( each => this.Inventory.Add( each ) );

            this.BanList = new List<string>();
            userData.BanList.ForEach( each => this.BanList.Add( each ) );
        }
    }

    public class CharacterModule : NancyModule
    {
        class GameResultBaseData
        {
            public int mapSize;
            public int randomSeed;
            public List<PlayerData> playerList = new List<PlayerData>();
        }

        public CharacterModule( ITokenizer tokenizer )
            : base( "/character" )
        {
            // 시뮬레이션 결과 요청
            Get["/simulation_result"] = parameters =>
            {
                // 일단 해당 유저의 id를 확인하고, 시뮬레이션 결과가 있는지 확인한다
                this.RequiresAuthentication();

                var resultTable = ResultTableDatabase.GetResultTable( this.Context.CurrentUser.UserName );

                if ( resultTable == null || resultTable.UnreadId == -1 )
                    return "nothing";

                // 해당 시뮬레이션 데이터를 가져온다
                var result = SimulationResultDatabase.GetSimulationResult( resultTable.UnreadId );

                if ( resultTable == null )
                    return "error";     // 조심해! 이거 500이나 뭐 그런 걸로 핸들링할까?

                // 시뮬레이션 결과 베이스 데이터를 구성한다
                GameResultBaseData baseData = new GameResultBaseData();
                baseData.mapSize = result.MapSize;
                baseData.randomSeed = result.Seed;

                result.PlayerList.ForEach( each => baseData.playerList.Add( each ) );

                // 읽은 애들은 삭제하자
                // 일단은 놔둠
                resultTable.ReadId.Add( resultTable.UnreadId );
                resultTable.UnreadId = -1;   // default value

                Debug.Assert( ResultTableDatabase.SetResultTable( resultTable ) );

                // 전송한다
                return JsonConvert.SerializeObject( baseData ); ;
            };

            Get["/update"] = parameters =>
            {
                // 캐릭터의 최신 정보 받기

                // 일단 해당 유저의 id를 확인하고
                this.RequiresAuthentication();

                PlayerData playerData = PlayerDataDatabase.GetPlayerData( this.Context.CurrentUser.UserName );
                UserData userData = UserDataDatabase.GetUserData( this.Context.CurrentUser.UserName );

                return JsonConvert.SerializeObject( new ClientPlayerData( playerData, userData ) ); ;
            };

            Get["/levelup"] = parameters =>
            {
                try
                {
                    // 일단 해당 유저의 id를 확인하고
                    this.RequiresAuthentication();

                    PlayerData playerData = PlayerDataDatabase.GetPlayerData( this.Context.CurrentUser.UserName );

                    Debug.Assert( playerData != null );

                    Player player = new Player();
                    player.LoadPlayer( playerData );

                    if ( player.LevelUp( 1 ) )
                    {
                        // update playerData 
                        playerData.UpdateFromPlayer( player );

                        Debug.Assert( PlayerDataDatabase.SetPlayerData( playerData ) );

                        return "levelup";
                    }

                    return "need more exp";
                }
                catch( Exception e)
                {
                    Console.WriteLine( e );
                }

                return "fail";
            };

            Post["/increase_stat"] = parameters =>
            {
                // 일단 해당 유저의 id를 확인하고
                this.RequiresAuthentication();

                PlayerData playerData = PlayerDataDatabase.GetPlayerData( this.Context.CurrentUser.UserName );

                Player player = new Player();
                player.LoadPlayer( playerData );

                ushort[] request = JsonConvert.DeserializeObject<ushort[]>( this.Request.Form.stat ); 
                List<Tuple<StatType, ushort>> requestStats = new List<Tuple<StatType, ushort>>();

                for ( int i = (int)StatType.Str; i < (int)StatType.Mov; ++i )
                {
                    if ( request[i] != 0 )
                        requestStats.Add( new Tuple<StatType, ushort>( (StatType)i, request[i] ) );
                }

                if ( player.SetBonusStats( requestStats ) )
                {
                    // update playerData
                    playerData.UpdateFromPlayer( player );

                    Debug.Assert( PlayerDataDatabase.SetPlayerData( playerData ) );

                    return "stats changed";
                }

                return "failed";
            };

            Get["/rank"] = parameters =>
            {
                // 일단 해당 유저의 id를 확인하고
                this.RequiresAuthentication();

                long rank = RedisManager.GetRank( this.Context.CurrentUser.UserName );

                return rank;
            };
        }
    }
}