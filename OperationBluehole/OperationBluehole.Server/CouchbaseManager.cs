using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;

using Nancy.Security;

using Couchbase;
using Couchbase.Extensions;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;

using OperationBluehole.Content;

namespace OperationBluehole.Server
{
    public class AccountInfo
    {
        [JsonProperty("name")]
        public string AccountName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public AccountInfo(string name, string password)
        {
            this.AccountName = name;
            this.Password = password;
        }
    }

    public class UserIdentity : IUserIdentity
    {
        [JsonProperty("name")]
        public string UserName { get; set; }

        [JsonProperty("claims")]
        public IEnumerable<string> Claims { get; set; }

        /*
        [JsonProperty("characterId")]
        public ulong CharacterId { get; set; }
        */ 
    }

    public class PlayerDataSource
    {
        [JsonProperty( "playerId" )]
        public string PlayerId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("exp")]
        public uint Exp { get; set; }

        [JsonProperty("stat")]
        public List<ushort> Stat { get; set; }

        [JsonProperty("skill")]
        public List<ushort> Skill { get; set; }

        // 전체 인벤토리
        [JsonProperty("inventory")]
        public List<uint> Inventory { get; set; }

        // 장착한 장비
        [JsonProperty("equipment")]
        public List<uint> Equipment { get; set; }

        // 장착한 소모품
        [JsonProperty("consumable")]
        public List<uint> Consumable { get; set; }

        [JsonProperty("battleStyle")]
        public byte BattleStyle { get; set; }

        [JsonProperty( "banList" )]
        public List<string> BanList { get; set; }
    }

    public class ResultTable
    {
        [JsonProperty( "playerId" )]
        public string PlayerId { get; set; }

        [JsonProperty( "unread" )]
        public long UnreadId { get; set; }

        [JsonProperty( "read" )]
        public List<long> ReadId { get; set; }
    }

    public class SimulationResult
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        // 참가한 player id 목록
        [JsonProperty("playerData")]
        public List<PlayerData> PlayerList { get; set; }

        // 맵 크기
        [JsonProperty( "mapSize" )]
        public int MapSize { get; set; }

        // 게임 결과를 확인한 플레이어 - 전부 확인하면 지울 수 있도록? 아니면 아예 무조건 타임아웃? 적절히 혼합?
        [JsonProperty( "checkedPlayer" )]
        public List<ulong> CheckedPlayer { get; set; }

        // 시뮬레이션에 사용한 random seed 값
        [JsonProperty("seed")]
        public int Seed { get; set; }
    }

    public static class CouchbaseManager
    {
        private readonly static CouchbaseClient _instance;

        static CouchbaseManager()
        {
            _instance = new CouchbaseClient();
        }

        public static CouchbaseClient Client { get { return _instance; } }

        #region STORE JSON
        // 1. store Dictionary<string, object>
        // 자료를 저장할 때 하나의 dictionary 안에 object로 다 넣고
        // 
        // 

        /*
        var user1 = new Dictionary<string, object>
		{
		    { "username", "jzablocki" },
		    { "preferences", new Dictionary<string, object>
		        {
		            { "theme",  "green"},
		            { "timezone",  "EST" }
		        }
		    }
		};
        */

        /*
        var result = client.StoreDictionary(StoreMode.Set, "user_1", user1);
        if (result.Item1) 
        {
           var dict = client.GetDictionary("user_1").Item4;
           Console.WriteLine(dict); //should be output of Dictionary.ToString()
        }
        */

        public static Tuple<bool, int, string> StoreDictionary(
            this ICouchbaseClient client,
            StoreMode storeMode,
            string key,
            Dictionary<string, object> dictionary)
        {
            var json = JsonConvert.SerializeObject(dictionary);
            var result = client.ExecuteStore(storeMode, key, json);

            if (!result.Success)
            {
                if (result.Exception != null) throw result.Exception;

                return Tuple.Create(false, result.StatusCode.HasValue ? result.StatusCode.Value : -1, result.Message);
            }

            return Tuple.Create(true, 0, string.Empty);
        }

        public static Tuple<bool, int, string, Dictionary<string, object>> GetDictionary(this ICouchbaseClient client, string key)
        {
            var result = client.ExecuteGet<string>(key);

            if (!result.Success)
            {
                if (result.Exception != null) throw result.Exception;

                return Tuple.Create<bool, int, string, Dictionary<string, object>>
                            (false, result.StatusCode.HasValue ? result.StatusCode.Value : -1, result.Message, null);
            }

            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.Value);
            return Tuple.Create(true, 0, string.Empty, dict);
        }


        // 2. Dynamic type을 활용하는 것
        // 일단 보류
        // Dynamic type 공부한 후에 사용하자

        public static Tuple<bool, int, string> StoreDynamic(this ICouchbaseClient client, StoreMode storeMode, string key, ExpandoObject obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var result = client.ExecuteStore(storeMode, key, json);

            if (!result.Success)
            {
                if (result.Exception != null) throw result.Exception as Exception;

                return Tuple.Create(false, result.StatusCode.HasValue ? result.StatusCode.Value : -1, result.Message);
            }

            return Tuple.Create(true, 0, string.Empty);
        }

        public static Tuple<bool, int, string, ExpandoObject> GetDynamic(this ICouchbaseClient client, string key)
        {
            var result = client.ExecuteGet<string>(key);

            if (!result.Success)
            {
                if (result.Exception != null) throw result.Exception;

                return Tuple.Create<bool, int, string, ExpandoObject>
                            (false, result.StatusCode.HasValue ? result.StatusCode.Value : -1, result.Message, null);
            }

            var obj = JsonConvert.DeserializeObject<ExpandoObject>(result.Value);
            return Tuple.Create(true, 0, string.Empty, obj);
        }

        /*
        dynamic user2 = new ExpandoObject();
        user2.Username = "jzablocki";
        user2.Preferences = new ExpandoObject();
        user2.Preferences.Theme = "green";
        user2.Preferences.TimeZone = "EST";

        client.StoreDynamic(StoreMode.Set, "user_2", user2 as ExpandoObject);
        var getResult = client.GetDynamic("user_2");
        if (getResult.Item1)
        {
            dynamic item = getResult.Item4;
            Console.WriteLine(item.Preferences.Theme);
        }
        */
        #endregion
    }

    public static class AccountInfoDatabase
    {
        public const string PREFIX = "Account ";

        public static AccountInfo GetAccountInfo(string userName)
        {
            return CouchbaseManager.Client.GetJson<AccountInfo>(PREFIX + userName);
        }

        public static bool SetAccountInfo(AccountInfo info )
        {
            return CouchbaseManager.Client.StoreJson(StoreMode.Set, PREFIX + info.AccountName, info);
        }
    }

    public static class UserIdentityDatabase
    {
        public const string PREFIX = "UserIdentity ";

        public static UserIdentity GetUserIdentity(string userName)
        {
            return CouchbaseManager.Client.GetJson<UserIdentity>(PREFIX + userName);
        }

        public static bool SetUserIdentity(UserIdentity userIdentity)
        {
            return CouchbaseManager.Client.StoreJson(StoreMode.Set, PREFIX + userIdentity.UserName, userIdentity);
        }

        public static IUserIdentity ValidateUser(string userName, string password)
        {
            var client = CouchbaseManager.Client;

            var account = AccountInfoDatabase.GetAccountInfo(userName);

            if (account == null || !account.Password.Equals(password))
                return null;

            var userIdentity = client.GetJson<UserIdentity>(PREFIX + userName);
            var claims = userIdentity.Claims;

            return new UserIdentity { UserName = account.AccountName, Claims = claims };
        }

        // Nancy module에서 token을 사용할 때
        // context의 UserInterface에 포함되는 내용은 Nancy에서 제공하는 IUserIdentity를 따른다
        // 그래서 context로 접근해서는 palyerId를 얻을 수 없음
        // 방법은 claim에 playerId를 끼워 넣는 것과
        // 아니면 userName과 playerData의 key값을 같은 걸로 사용 - userName을 이용해서 검색 - 하는 방법 두 가지
        // 일단은 후자로 구현하지만... 구리다...매우 구리다
    }

    public static class PlayerDataDatabase
    {
        public const string PREFIX = "PlayerData ";

        public static PlayerDataSource GetPlayerData( string playerId )
        {
            return CouchbaseManager.Client.GetJson<PlayerDataSource>( PREFIX + playerId );
        }

        public static bool SetPlayerData( PlayerDataSource data )
        {
            return CouchbaseManager.Client.StoreJson(StoreMode.Set, PREFIX + data.PlayerId, data);
        }
    }

    public static class ResultTableDatabase
    {
        public const string PREFIX = "ResultTable ";

        public static ResultTable GetResultTable( string playerId )
        {
            return CouchbaseManager.Client.GetJson<ResultTable>( PREFIX + playerId );
        }

        public static bool SetResultTable( ResultTable data )
        {
            return CouchbaseManager.Client.StoreJson( StoreMode.Set, PREFIX + data.PlayerId, data );
        }
    }

    public static class SimulationResultDatabase
    {
        public const string PREFIX = "SimulationResult ";

        public static SimulationResult GetSimulationResult( long resultIdx )
        {
            return CouchbaseManager.Client.GetJson<SimulationResult>( PREFIX + resultIdx );
        }

        public static bool SetSimulationResult( SimulationResult data )
        {
            return CouchbaseManager.Client.StoreJson( StoreMode.Set, PREFIX + data.Id, data );
        }
    }
}