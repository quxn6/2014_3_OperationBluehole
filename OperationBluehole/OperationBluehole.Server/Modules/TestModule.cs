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
                            0, 1, 5, 5, 5, 5, 5, 5, 5,
                        }
                    },
                    { "skil", new List<ushort>
                        {
                            (ushort)SkillId.Punch,
                        }
                    },
                    { "inventory", new List<uint>
                        {
                            
                        }
                    },
                    { "items", new List<uint>
                        {
                            
                        }
                    },
                    { "equipment", new List<uint>
                        {
                            
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
                var testData = new PlayerDataSource
                {
                    PlayerId = "0",
                    Name = "test",
                    Exp = 4,
                    Stat = new List<ushort> {1, 1, 1, 1, 1, 1, 1, 1,},
                    Skill = new List<ushort> {0, 1, 2,},
                    Inventory = new List<uint> {2, 4, 6, 8, 10,},
                    Consumable = new List<uint> {4, 6, 10,},
                    Equipment = new List<uint> {2, 8,}, 
                    BattleStyle = 0,
                };

                return PlayerDataDatabase.SetPlayerData(testData);
            };

            Get["/load_test_class"] = parameters =>
            {
                var savedData = PlayerDataDatabase.GetPlayerData("test");

                string result = "";

                result += "id" + savedData.PlayerId;
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

                var data = PlayerDataDatabase.GetPlayerData( "102" );
                if ( data != null )
                    player[0].LoadPlayer( data.ConvertToPlayerData() );

                data = PlayerDataDatabase.GetPlayerData( "103" );
                if ( data != null )
                    player[1].LoadPlayer( data.ConvertToPlayerData() );

                data = PlayerDataDatabase.GetPlayerData( "104" );
                if ( data != null )
                    player[2].LoadPlayer( data.ConvertToPlayerData() );

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

                MatchingManager.RegisterPlayer( "101", 0 );
                MatchingManager.RegisterPlayer( "102", 0 );
                MatchingManager.RegisterPlayer( "103", 0 );
                MatchingManager.RegisterPlayer( "104", 0 );

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
                var result = AccountInfoDatabase.SetAccountInfo(new AccountInfo("wooq", "next!!@@##$$"));

                if ( result )
                    result = AccountInfoDatabase.SetAccountInfo(new AccountInfo("quxn6", "next!!@@##$$")); 

                if (result)
                    result = AccountInfoDatabase.SetAccountInfo(new AccountInfo("yksera", "next!!@@##$$"));

                if ( result )
                    result = AccountInfoDatabase.SetAccountInfo( new AccountInfo( "sm9", "next!!@@##$$" ) ); 

                // user identity
                if (result)
                    result = UserIdentityDatabase.SetUserIdentity(new UserIdentity { UserName = "wooq", Claims = new List<string> { "admin", } });

                if (result)
                    result = UserIdentityDatabase.SetUserIdentity(new UserIdentity { UserName = "quxn6", Claims = new List<string> { "admin", } }); 

                if (result)
                    result = UserIdentityDatabase.SetUserIdentity(new UserIdentity { UserName = "yksera", Claims = new List<string> { "admin", } });

                if ( result )
                    result = UserIdentityDatabase.SetUserIdentity( new UserIdentity { UserName = "sm9", Claims = new List<string> { "admin", } } ); 

                // player data
                if (result)
                    result = PlayerDataDatabase.SetPlayerData( new PlayerDataSource
                    {
                        PlayerId = "wooq",
                        Name = "wooq",
                        Exp = 4,
                        Stat = new List<ushort> { 0, 1, 5, 5, 5, 5, 5, 5, 5, },
                        Skill = new List<ushort> { (ushort)SkillId.Punch, },
                        Inventory = new List<uint> { },
                        Consumable = new List<uint> { },
                        Equipment = new List<uint> { },
                        BattleStyle = (byte)BattleStyle.AGGRESSIVE,
                        BanList = new List<string> { },
                    } );

                if ( result )
                    result = ResultTableDatabase.SetResultTable( new ResultTable 
                    {
                        PlayerId = "wooq", 
                        ReadId = new List<long> { }, 
                        UnreadId = -1 
                    } );

                if (result)
                    result = PlayerDataDatabase.SetPlayerData( new PlayerDataSource
                    {
                        PlayerId = "quxn6",
                        Name = "quxn6",
                        Exp = 4,
                        Stat = new List<ushort> { 0, 3, 5, 5, 5, 15, 5, 5, 5, },
                        Skill = new List<ushort> { (ushort)SkillId.Punch, (ushort)SkillId.MagicArrow, (ushort)SkillId.Heal, },
                        Inventory = new List<uint> { (uint)ItemCode.MpPotion_S },
                        Consumable = new List<uint> { (uint)ItemCode.MpPotion_S },
                        Equipment = new List<uint> {},
                        BattleStyle = (byte)BattleStyle.DEFENSIVE,
                        BanList = new List<string> { },
                    } );

                if ( result )
                    result = ResultTableDatabase.SetResultTable( new ResultTable
                    {
                        PlayerId = "quxn6",
                        ReadId = new List<long> { },
                        UnreadId = -1
                    } );

                if (result)
                    result = PlayerDataDatabase.SetPlayerData( new PlayerDataSource
                    {
                        PlayerId = "yksera",
                        Name = "yksera",
                        Exp = 4,
                        Stat = new List<ushort> { 0, 3, 15, 5, 5, 5, 5, 5, 5, },
                        Skill = new List<ushort> { (ushort)SkillId.Slash, },
                        Inventory = new List<uint> { (uint)ItemCode.HpPotion_S, (uint)ItemCode.HpPotion_S, (uint)ItemCode.Sword_Test, },
                        Consumable = new List<uint> { (uint)ItemCode.HpPotion_S, (uint)ItemCode.HpPotion_S, },
                        Equipment = new List<uint> { (uint)ItemCode.Sword_Test, },
                        BattleStyle = (byte)BattleStyle.AGGRESSIVE,
                        BanList = new List<string> { },
                    } );

                if ( result )
                    result = ResultTableDatabase.SetResultTable( new ResultTable
                    {
                        PlayerId = "yksera",
                        ReadId = new List<long> { },
                        UnreadId = -1
                    } );

                if ( result )
                    result = PlayerDataDatabase.SetPlayerData( new PlayerDataSource
                    {
                        PlayerId = "sm9",
                        Name = "sm9",
                        Exp = 0,
                        Stat = new List<ushort> { 0, 3, 10, 10, 5, 5, 5, 5, 5, },
                        Skill = new List<ushort> { (ushort)SkillId.Punch, (ushort)SkillId.Heal, },
                        Inventory = new List<uint> { (uint)ItemCode.MpPotion_S, },
                        Consumable = new List<uint> { (uint)ItemCode.MpPotion_S, },
                        Equipment = new List<uint> {},
                        BattleStyle = (byte)BattleStyle.AGGRESSIVE,
                        BanList = new List<string> { },
                    } );

                if ( result )
                    result = ResultTableDatabase.SetResultTable( new ResultTable
                    {
                        PlayerId = "sm9",
                        ReadId = new List<long> { },
                        UnreadId = -1
                    } );

                if (result)
                    return "done!";

                return "FAIL";
            };

            Post["/"] = x =>
            {
                var userName = (string)this.Request.Form.UserName;
                var password = (string)this.Request.Form.Password;

                var userIdentity = UserIdentityDatabase.ValidateUser(userName, password);

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

                var playerData = PlayerDataDatabase.GetPlayerData(this.Context.CurrentUser.UserName);

                Console.WriteLine("Character Info");
                Console.WriteLine("name : " + playerData.Name + " / Id : " + playerData.PlayerId);
                Console.WriteLine("Lev : " + playerData.Stat[(int)StatType.Lev] + " / Exp : " + playerData.Exp);

                return "Yay! You are authorized!";
            };
        }
    }
}