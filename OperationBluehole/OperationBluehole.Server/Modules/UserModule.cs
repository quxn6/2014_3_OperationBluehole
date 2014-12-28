using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace OperationBluehole.Server.Modules
{
    using Nancy;
    using Nancy.ModelBinding;
    using Nancy.Security;
    using Nancy.Authentication.Token;

    using OperationBluehole.Content;
    using OperationBluehole.Database;

    // 가입, 로그인 등 사용자 인증 작업을 처리
    public class UserModule : NancyModule
    {
        public UserModule( ITokenizer tokenizer )
            : base("/user")
        {
            // 가입
            // 이메일, 비밀번호, 
            Post["/signup"] = parameters =>
            {
                var client = CouchbaseManager.Client;

                string userId = Request.Form.userId;
                string password = Request.Form.password;
                string playerName = Request.Form.playername;

                // if ( AccountInfoDatabase.SetAccountInfo( new AccountInfo( userId, password ) ) )
                var task = PostgresqlManager.SetAccountInfo(userId, password);
                task.Wait();

                if (task.Result)
                {
                    // user identity
                    // UserIdentityDatabase.SetUserIdentity( new UserIdentity { UserName = userId, Claims = new List<string> { "user", } } );

                    // user data
                    UserDataDatabase.SetUserData( new UserData {
                        UserId = userId,
                        Gold = 0,
                        Inventory = new List<uint> { },
                        Token = new List<ItemToken> { },
                        BanList = new List<string> { },
                    } );
                    
                    // player data
                    PlayerDataDatabase.SetPlayerData( new PlayerData
                    {
                        pId = userId,
                        name = playerName,
                        exp = 0,
                        stats = new ushort[] { 1, 5, 5, 5, 5, 5, 5, 5, },
                        skills = new List<SkillId> { SkillId.Punch, },
                        consumables = new List<ItemCode> { ItemCode.HpPotion_S, },
                        equipments = new List<ItemCode> { ItemCode.Sword_Test, },
                        battleStyle = BattleStyle.AGGRESSIVE,
                    } );
                    
                    // result table
                    ResultTableDatabase.SetResultTable( new ResultTable
                    {
                        PlayerId = userId,
                        ReadId = new List<long> { },
                        UnreadId = -1
                    } );

                    // ranking list
                    Debug.Assert( RedisManager.RegisterPlayerRank( userId ) );

                    return "success";
                }
                else
                {
                    // 조심해!!!!!!!
                    // RDM 매번 초기화하기 힘들어서 이미 존재하는 아이디어도 덮어쓰기로 가입

                    // user data
                    UserDataDatabase.SetUserData( new UserData
                    {
                        UserId = userId,
                        Gold = 0,
                        Inventory = new List<uint> { },
                        Token = new List<ItemToken> { },
                        BanList = new List<string> { },
                    } );

                    // player data
                    PlayerDataDatabase.SetPlayerData( new PlayerData
                    {
                        pId = userId,
                        name = playerName,
                        exp = 0,
                        stats = new ushort[] { 1, 5, 5, 5, 5, 5, 5, 5, },
						skills = new List<SkillId> { SkillId.Punch, },
						consumables = new List<ItemCode> { ItemCode.HpPotion_S, },
						equipments = new List<ItemCode> { ItemCode.Sword_Test, },
                        battleStyle = BattleStyle.AGGRESSIVE,
                    } );

                    // result table
                    ResultTableDatabase.SetResultTable( new ResultTable
                    {
                        PlayerId = userId,
                        ReadId = new List<long> { },
                        UnreadId = -1
                    } );

                    // ranking list
                    // Debug.Assert( RedisManager.RegisterPlayerRank( userId ) );
                    RedisManager.RegisterPlayerRank( userId );

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

                // var userIdentity = UserIdentityDatabase.ValidateUser( userName, password );
                var task = PostgresqlManager.ValidateUser(userName, password);
                task.Wait();
                var userIdentity = task.Result;

                if ( userIdentity == null )
                {
                    return HttpStatusCode.Unauthorized;
                }

                var token = tokenizer.Tokenize( userIdentity, Context );

                Console.WriteLine(userName + " : " + token);

                return new
                {
                    Token = token,
                };
            };

            // 탈퇴
            // 는 없다. 

            // 로그인 상태 확인
            Get["/valid_session"] = parameters =>
            {
                this.RequiresAuthentication();

                return "valid";
            };
        }
    }
}