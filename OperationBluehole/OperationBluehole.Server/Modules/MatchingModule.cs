using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OperationBluehole.Server.Module
{
    using Nancy;
    using Nancy.Security;
    using Nancy.Authentication.Token;

    // 사용자들이 신청하면 적절하게 매칭 시켜서 시뮬레이션 시킨다
    public class MatchingModule : NancyModule
    {
        public MatchingModule( ITokenizer tokenizer )
            : base( "/matching" )
        {
            Post["/register"] = parameters =>
            {
                // var difficulty = this.Bind<int>();
                int difficulty = Request.Form.difficulty;

                // 일단 해당 유저의 id를 확인하고
                this.RequiresAuthentication();

                var resultTable = ResultTableDatabase.GetResultTable( this.Context.CurrentUser.UserName );

                if ( resultTable == null )
                    return "nothing";

                // 해당 시뮬레이션 데이터를 가져온다
                var result = SimulationResultDatabase.GetSimulationResult( resultTable.UnreadId );

                MatchingManager.RegisterPlayer( this.Context.CurrentUser.UserName, difficulty );    // 리스트는 나중에 플레이어 데이터에 만들어서 추가할 것

                return "success";
            };
        }

    }
}