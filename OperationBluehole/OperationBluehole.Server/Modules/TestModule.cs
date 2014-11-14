using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Couchbase;
using Couchbase.Extensions;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;

namespace OperationBluehole.Server.Modules
{
    using Nancy;
    using Nancy.Security;
    using Nancy.Authentication.Token;

    using OperationBluehole.Content;

    // 한다! 테스트! 
    public class TestModule : NancyModule
    {
        public TestModule(ITokenizer tokenizer)
            : base("/test")
        {
            Post["/"] = parameters =>
            {
                return "default";
            };

            Get["/dict_save"] = parameters =>
            {
                var playerData = new Dictionary<string, object>
                {
                    { "id", 0 },
                    { "name", "quxn6" },
                    { "exp", 4 },
                    { "stat", new List<ushort>
                        {
                            1, 1, 1, 1, 1, 1, 1, 1,
                        }
                    },
                    { "skil", new List<ushort>
                        {
                            0, 1, 2,
                        }
                    },
                    { "inventory", new List<uint>
                        {
                            2, 4, 6, 8, 10,
                        }
                    },
                    { "items", new List<uint>
                        {
                            4, 6, 10,
                        }
                    },
                    { "equipment", new List<uint>
                        {
                            2, 8,
                        }
                    },
                    { "battle style", 0 },
                };

                var client = CouchbaseManager.Client;
                var result = client.StoreDictionary(StoreMode.Set, "testPlayer_0", playerData);

                if (result.Item1) 
                    return "success";
                
                return "fail";
            };

            Get["/dict_load"] = parameters =>
            {
                var client = CouchbaseManager.Client;
                var dict = client.GetDictionary( "testPlayer_0" ).Item4["name"];

                return dict.ToString();
            };

            Get["/save_test_class"] = parameters =>
            {
                var testData = new PlayerData
                {
                    Id = 0,
                    Name = "test",
                    Exp = 4,
                    Stat = new List<ushort> {1, 1, 1, 1, 1, 1, 1, 1,},
                    Skill = new List<ushort> {0, 1, 2,},
                    Inventory = new List<uint> {2, 4, 6, 8, 10,},
                    Consumable = new List<uint> {4, 6, 10,},
                    Equipment = new List<uint> {2, 8,}, 
                    BattleStyle = 0,
                };

                return CouchbaseManager.SetPlayerData(testData);
            };

            Get["/load_test_class"] = parameters =>
            {
                var savedData = CouchbaseManager.GetPlayerData( 0 );

                string result = "";

                result += "id" + savedData.Id;
                result += "name" + savedData.Name;
                result += "Exp" + savedData.Exp;
                result += "stat";
                savedData.Stat.ForEach(each => result += each + ",");

                return result;
            };
            
            Get["/simulation"] = parameters =>
            {
                ContentsPrepare.Init();

                Player[] player = { new Player(), new Player(), new Player() };
                player[0].LoadPlayer(102);
                player[1].LoadPlayer(103);
                player[2].LoadPlayer(104);

                Party users = new Party(PartyType.PLAYER, 10);
                foreach (Player p in player)
                    users.AddCharacter(p);

                DungeonMaster newMaster = new DungeonMaster();
                newMaster.Init(60, 4, users);

                return "turn : " + newMaster.Start();
            };

            Get["/matching_test"] = parameters =>
            {
                SimulationManger.Init();
                MatchingManager.Init();

                MatchingManager.RegisterPlayer(101, 0, new List<ulong>());
                MatchingManager.RegisterPlayer(102, 0, new List<ulong>());
                MatchingManager.RegisterPlayer(103, 0, new List<ulong>());
                MatchingManager.RegisterPlayer(104, 0, new List<ulong>());

                return "check";
            };

        }
    }
    
    public class AuthModule : NancyModule
    {
        public AuthModule(ITokenizer tokenizer)
            : base("/auth")
        {
            Get["/init"] = parameters =>
            {
                var client = CouchbaseManager.Client;

                // account
                var result = client.StoreJson(StoreMode.Set, AccountInfo.PREFIX + "wooq", new AccountInfo("wooq", "next!!@@##$$"));

                if ( result )
                    result = client.StoreJson(StoreMode.Set, AccountInfo.PREFIX + "quxn6", new AccountInfo("quxn6", "next!!@@##$$"));

                if (result)
                    result = client.StoreJson(StoreMode.Set, AccountInfo.PREFIX + "yksera", new AccountInfo("yksera", "next!!@@##$$"));

                // user identity
                if (result)
                    result = client.StoreJson(StoreMode.Set, UserIdentity.PREFIX + "wooq", new UserIdentity{ UserName = "wooq", Claims = new List<string>{ "admin", } } );

                if (result)
                    result = client.StoreJson(StoreMode.Set, UserIdentity.PREFIX + "quxn6", new UserIdentity { UserName = "quxn6", Claims = new List<string> { "admin", } });

                if (result)
                    result = client.StoreJson(StoreMode.Set, UserIdentity.PREFIX + "yksera", new UserIdentity { UserName = "yksera", Claims = new List<string> { "admin", } });

                if (result)
                    return "done!";

                return "FAIL";
            };

            Post["/"] = x =>
            {
                var userName = (string)this.Request.Form.UserName;
                var password = (string)this.Request.Form.Password;

                var userIdentity = UserDatabase.ValidateUser(userName, password);

                if (userIdentity == null)
                {
                    return HttpStatusCode.Unauthorized;
                }

                var token = tokenizer.Tokenize(userIdentity, Context);

                return new
                {
                    Token = token,
                };
            };

            Get["/validation"] = _ =>
            {
                this.RequiresAuthentication();
                return "Yay! You are authenticated!";
            };

            Get["/admin"] = _ =>
            {
                this.RequiresAuthentication();
                this.RequiresClaims(new[] { "admin" });

                Console.WriteLine("name : " + this.Context.CurrentUser.UserName);

                return "Yay! You are authorized!";
            };
        }
    }
}