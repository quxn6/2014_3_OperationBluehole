using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OperationBluehole.Server.Modules
{
    using Nancy;
    using Nancy.ModelBinding;
    using Nancy.Security;
    using Nancy.Authentication.Token;

    using OperationBluehole.Content;

    // 가입, 로그인 등 사용자 인증 작업을 처리
    public class UserModule : NancyModule
    {
        public UserModule( ITokenizer tokenizer )
            : base("/user")
        {
            // 가입
            // 이메일, 비밀번호, 
            Post["/signin"] = parameters =>
            {
                var client = CouchbaseManager.Client;

                string userId = Request.Form.userId;
                string password = Request.Form.password;
                string playerName = Request.Form.playername;

                if ( AccountInfoDatabase.SetAccountInfo( new AccountInfo( userId, password ) ) )
                {
                    UserIdentityDatabase.SetUserIdentity( new UserIdentity { UserName = userId, Claims = new List<string> { "user", } } );
                    
                    PlayerDataDatabase.SetPlayerData( new PlayerDataSource
                    {
                        PlayerId = userId,
                        Name = playerName,
                        Exp = 0,
                        Stat = new List<ushort> { 0, 0, 0, 0, 0, 0, 0, 0, 0, },
                        Skill = new List<ushort> { },
                        Inventory = new List<uint> { },
                        Consumable = new List<uint> { },
                        Equipment = new List<uint> { },
                        BattleStyle = (byte)BattleStyle.AGGRESSIVE,
                        BanList = new List<string> { },
                    } );
                    
                    ResultTableDatabase.SetResultTable( new ResultTable
                    {
                        PlayerId = userId,
                        ReadId = new List<long> { },
                        UnreadId = -1
                    } );

                    return "success";
                }

                return "fail";
            };

            // 로그인
            // 이메일과 비밀번호를 전송받아서
            // 저장된 값과 비교해보고
            // 같으면 쿠키 발급
            Post["/login"] = parameters =>
            {
                var userName = (string)this.Request.Form.UserName;
                var password = (string)this.Request.Form.Password;

                var userIdentity = UserIdentityDatabase.ValidateUser( userName, password );

                if ( userIdentity == null )
                {
                    return HttpStatusCode.Unauthorized;
                }

                var token = tokenizer.Tokenize( userIdentity, Context );

                return new
                {
                    Token = token,
                };
            };

            // 탈퇴
            // 는 없다. 

            // 로그인 상태 확인
            Post["/valid_session"] = parameters =>
            {
                this.RequiresAuthentication();

                return "valid";
            };
        }
    }
}