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

    // 플레이어의 캐릭터를 조작( 장비 설정이나 능력치 배분과 같은... )

    private class ClientPlayerData
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

        public ClientPlayerData( PlayerDataSource source )
        {
            this.Name = source.Name;

            this.Exp = source.Exp;
            this.StatPoints = source.StatPoints;

            this.Stat = new List<ushort>();
            source.Stat.ForEach( each => this.Stat.Add( each ) );

            this.Skill = new List<ushort>();
            source.Stat.ForEach( each => this.Skill.Add( each ) );

            this.Gold = source.Gold;

            this.Inventory = new List<uint>();
            source.Inventory.ForEach( each => this.Inventory.Add( each ) );

            this.Token = new List<ItemToken>();
            source.Token.ForEach( each => this.Token.Add( each ) );

            this.Equipment = new List<uint>();
            source.Equipment.ForEach( each => this.Equipment.Add( each ) );

            this.Consumable = new List<uint>();
            source.Consumable.ForEach( each => this.Consumable.Add( each ) );

            this.BattleStyle = source.BattleStyle;

            this.BanList = new List<string>();
            source.BanList.ForEach( each => this.BanList.Add( each ) );
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
                // PlayerDataSource를 보낸다

                // 일단 해당 유저의 id를 확인하고
                this.RequiresAuthentication();

                PlayerDataSource playerData = PlayerDataDatabase.GetPlayerData( this.Context.CurrentUser.UserName );

                return JsonConvert.SerializeObject( new ClientPlayerData( playerData ) ); ;
            };

            Get["/levelup"] = parameters =>
            {
                // 일단 해당 유저의 id를 확인하고
                this.RequiresAuthentication();

                PlayerDataSource playerData = PlayerDataDatabase.GetPlayerData( this.Context.CurrentUser.UserName );

                uint expRequired = Content.Config.GetExpRequired( playerData.Stat[(int)StatType.Lev] );
                if ( playerData.Exp >= expRequired )
                {
                    playerData.Exp -= expRequired;
                    ++playerData.Stat[(int)StatType.Lev];

                    playerData.StatPoints += Content.Config.BONUS_SKILL_POINTS_EACH_LEVELUP;

                    Debug.Assert( PlayerDataDatabase.SetPlayerData( playerData ) );

                    return "levelup";
                }

                return "need more exp";
            };

            Post["/increase_stat"] = parameters =>
            {
                // 일단 해당 유저의 id를 확인하고
                this.RequiresAuthentication();

                int stat = Request.Form.stat;

                PlayerDataSource playerData = PlayerDataDatabase.GetPlayerData( this.Context.CurrentUser.UserName );

                if ( playerData.StatPoints <= 0 )
                    return "no point";

                if ( stat <= (int)StatType.Lev || stat >= (int)StatType.StatCount )
                    return "wrong stat";

                ++playerData.Stat[stat];

                Debug.Assert( PlayerDataDatabase.SetPlayerData( playerData ) );

                return "increased";
            };
        }
    }
}