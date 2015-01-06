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

    using OperationBluehole.Content;
    using OperationBluehole.Database;
    using RabbitMQ.Client;
    using System.Text;
    using LitJson;

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

                using ( var connection = MessageManager.connectionFactory.CreateConnection() )
                {
                    using ( var channel = connection.CreateModel() )
                    {
                        channel.QueueDeclare( "matching_lobby_queue", true, false, false, null );

                        var message = JsonMapper.ToJson( new Dictionary<string, object> { { "playerId", this.Context.CurrentUser.UserName}, { "difficulty", difficulty } } );
                        var body = Encoding.UTF8.GetBytes( message );

                        var properties = channel.CreateBasicProperties();
                        properties.SetPersistent( true );
						properties.Type = "REG";

						channel.BasicPublish( "", "matching_lobby_queue", properties, body );
                        Console.WriteLine( " [x] Sent {0}", message );
                    }
                }

                return "success";
            };
        }

    }
}