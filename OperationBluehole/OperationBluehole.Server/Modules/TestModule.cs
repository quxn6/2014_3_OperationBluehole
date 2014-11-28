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

            Get["/simulation"] = parameters =>
            {
                ContentsPrepare.Init();

                Player[] player = { new Player(), new Player(), new Player() };

                var data = PlayerDataDatabase.GetPlayerData( "102" );
                if ( data != null )
                    player[0].LoadPlayer( data );

                data = PlayerDataDatabase.GetPlayerData( "103" );
                if ( data != null )
                    player[1].LoadPlayer( data );

                data = PlayerDataDatabase.GetPlayerData( "104" );
                if ( data != null )
                    player[2].LoadPlayer( data );

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

                // user data
                if ( result )
                    result = UserDataDatabase.SetUserData( new UserData 
                    {
                        UserId = "wooq",
                        Gold = 0,
                        Inventory = new List<uint> { },
                        Token = new List<ItemToken> { },
                        BanList = new List<string> { },
                    } );

                if ( result )
                    result = UserDataDatabase.SetUserData( new UserData
                    {
                        UserId = "quxn6",
                        Gold = 0,
                        Inventory = new List<uint> { },
                        Token = new List<ItemToken> { },
                        BanList = new List<string> { },
                    } );

                if ( result )
                    result = UserDataDatabase.SetUserData( new UserData
                    {
                        UserId = "yksera",
                        Gold = 0,
                        Inventory = new List<uint> { },
                        Token = new List<ItemToken> { },
                        BanList = new List<string> { },
                    } );

                if ( result )
                    result = UserDataDatabase.SetUserData( new UserData
                    {
                        UserId = "sm9",
                        Gold = 0,
                        Inventory = new List<uint> { },
                        Token = new List<ItemToken> { },
                        BanList = new List<string> { },
                    } );

                // player data
                if (result)
                    result = PlayerDataDatabase.SetPlayerData( new PlayerData
                    {
                        pId = "wooq",
                        name = "wooq",
                        exp = 4,
                        stats = new ushort[] { 0, 1, 5, 5, 5, 5, 5, 5, 5, },
                        skills = new List<SkillId> { SkillId.Punch, },
                        equipments = new List<ItemCode> { },
                        consumables = new List<ItemCode> { },
                        battleStyle = BattleStyle.AGGRESSIVE,
                    } );

                if ( result )
                    result = ResultTableDatabase.SetResultTable( new ResultTable 
                    {
                        PlayerId = "wooq", 
                        ReadId = new List<long> { }, 
                        UnreadId = -1 
                    } );

                if (result)
                    result = PlayerDataDatabase.SetPlayerData( new PlayerData
                    {
                        pId = "quxn6",
                        name = "quxn6",
                        exp = 4,
                        stats = new ushort[] { 0, 3, 5, 5, 5, 15, 5, 5, 5, },
                        skills = new List<SkillId> { SkillId.Punch, SkillId.MagicArrow, SkillId.Heal, },
                        equipments = new List<ItemCode> { },
                        consumables = new List<ItemCode> { ItemCode.MpPotion_S },
                        battleStyle = BattleStyle.DEFENSIVE,
                    } );

                if ( result )
                    result = ResultTableDatabase.SetResultTable( new ResultTable
                    {
                        PlayerId = "quxn6",
                        ReadId = new List<long> { },
                        UnreadId = -1
                    } );

                if (result)
                    result = PlayerDataDatabase.SetPlayerData( new PlayerData
                    {
                        pId = "yksera",
                        name = "yksera",
                        exp = 4,
                        stats = new ushort[] { 0, 3, 15, 5, 5, 5, 5, 5, 5, },
                        skills = new List<SkillId> { SkillId.Slash, },
                        equipments = new List<ItemCode> { ItemCode.Sword_Test, },
                        consumables = new List<ItemCode> { ItemCode.HpPotion_S, ItemCode.HpPotion_S, },
                        battleStyle = BattleStyle.AGGRESSIVE,
                    } );

                if ( result )
                    result = ResultTableDatabase.SetResultTable( new ResultTable
                    {
                        PlayerId = "yksera",
                        ReadId = new List<long> { },
                        UnreadId = -1
                    } );

                if ( result )
                    result = PlayerDataDatabase.SetPlayerData( new PlayerData
                    {
                        pId = "sm9",
                        name = "sm9",
                        exp = 0,
                        stats = new ushort[] { 0, 3, 10, 10, 5, 5, 5, 5, 5, },
                        skills = new List<SkillId> { SkillId.Punch, SkillId.Heal, },
                        equipments = new List<ItemCode> { },
                        consumables = new List<ItemCode> { ItemCode.MpPotion_S, },
                        battleStyle = BattleStyle.AGGRESSIVE,
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
                Console.WriteLine("name : " + playerData.name + " / Id : " + playerData.pId);
                Console.WriteLine("Lev : " + playerData.stats[(int)StatType.Lev] + " / Exp : " + playerData.exp);

                return "Yay! You are authorized!";
            };
        }
    }
}