using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Database
{
    using Couchbase;
    using Couchbase.Extensions;
    using Enyim.Caching.Memcached;
    using Newtonsoft.Json;

    using OperationBluehole.Content;

    public class AccountInfo
    {
        [JsonProperty( "name" )]
        public string AccountName { get; set; }

        [JsonProperty( "password" )]
        public string Password { get; set; }

        public AccountInfo( string name, string password )
        {
            this.AccountName = name;
            this.Password = password;
        }
    }

    public class UserData
    {
        [JsonProperty( "UserId" )]
        public string UserId { get; set; }

        [JsonProperty( "gold" )]
        public uint Gold { get; set; }

        // 전체 인벤토리
        [JsonProperty( "inventory" )]
        public List<uint> Inventory { get; set; }

        [JsonProperty( "token" )]
        public List<ItemToken> Token { get; set; }

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
        [JsonProperty( "id" )]
        public long Id { get; set; }

        // 참가한 player id 목록
        [JsonProperty( "playerData" )]
        public List<PlayerData> PlayerList { get; set; }

        // 맵 크기
        [JsonProperty( "mapSize" )]
        public int MapSize { get; set; }

        // 게임 결과를 확인한 플레이어 - 전부 확인하면 지울 수 있도록? 아니면 아예 무조건 타임아웃? 적절히 혼합?
        [JsonProperty( "checkedPlayer" )]
        public List<ulong> CheckedPlayer { get; set; }

        // 시뮬레이션에 사용한 random seed 값
        [JsonProperty( "seed" )]
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
        public static Tuple<bool, int, string> StoreDictionary(
            this ICouchbaseClient client,
            StoreMode storeMode,
            string key,
            Dictionary<string, object> dictionary )
        {
            var json = JsonConvert.SerializeObject( dictionary );
            var result = client.ExecuteStore( storeMode, key, json );

            if ( !result.Success )
            {
                if ( result.Exception != null ) throw result.Exception;

                return Tuple.Create( false, result.StatusCode.HasValue ? result.StatusCode.Value : -1, result.Message );
            }

            return Tuple.Create( true, 0, string.Empty );
        }

        public static Tuple<bool, int, string, Dictionary<string, object>> GetDictionary( this ICouchbaseClient client, string key )
        {
            var result = client.ExecuteGet<string>( key );

            if ( !result.Success )
            {
                if ( result.Exception != null ) throw result.Exception;

                return Tuple.Create<bool, int, string, Dictionary<string, object>>
                            ( false, result.StatusCode.HasValue ? result.StatusCode.Value : -1, result.Message, null );
            }

            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>( result.Value );
            return Tuple.Create( true, 0, string.Empty, dict );
        }
        #endregion
    }

    public static class AccountInfoDatabase
    {
        public const string PREFIX = "Account ";

        public static AccountInfo GetAccountInfo( string userName )
        {
            return CouchbaseManager.Client.GetJson<AccountInfo>( PREFIX + userName );
        }

        public static bool SetAccountInfo( AccountInfo info )
        {
            return CouchbaseManager.Client.StoreJson( StoreMode.Add, PREFIX + info.AccountName, info );
        }
    }

    public static class UserDataDatabase
    {
        public const string PREFIX = "UserData ";

        public static UserData GetUserData( string userId )
        {
            return CouchbaseManager.Client.GetJson<UserData>( PREFIX + userId );
        }

        public static bool SetUserData( UserData data )
        {
            return CouchbaseManager.Client.StoreJson( StoreMode.Set, PREFIX + data.UserId, data );
        }
    }

    public static class PlayerDataDatabase
    {
        public const string PREFIX = "PlayerData ";

        public static PlayerData GetPlayerData( string playerId )
        {
            return CouchbaseManager.Client.GetJson<PlayerData>( PREFIX + playerId );
        }

        public static bool SetPlayerData( PlayerData data )
        {
            return CouchbaseManager.Client.StoreJson( StoreMode.Set, PREFIX + data.pId, data );
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
