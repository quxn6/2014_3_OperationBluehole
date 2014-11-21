using System;
using System.Collections.Generic;
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
            Get["/result"] = parameters =>
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

                if ( ResultTableDatabase.SetResultTable( resultTable ) )
                    return "error"; // 유저한데 에러를 던질 필요는 없는데...

                // 전송한다
                return JsonConvert.SerializeObject( baseData ); ;
            };
        }
    }
}