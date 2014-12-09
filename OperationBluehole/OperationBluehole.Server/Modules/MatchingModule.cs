using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OperationBluehole.Server.Module
{
    using Nancy;
    using Nancy.ModelBinding;
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
                // 일단 해당 유저의 id를 확인하고
                this.RequiresAuthentication();
                // this.RequiresClaims( new[] { "admin" } );

                // var difficulty = this.Bind<int>( "difficulty" );
                // int difficulty = this.Bind();
                int difficulty = Request.Form.difficulty;

                var resultTable = ResultTableDatabase.GetResultTable( this.Context.CurrentUser.UserName );

                if ( resultTable != null && resultTable.UnreadId != -1  )
                    return "not prepared";

                MatchingManager.RegisterPlayer( this.Context.CurrentUser.UserName, difficulty );

                return "success";
            };
        }

    }
}