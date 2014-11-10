using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Couchbase;
using Couchbase.Extensions;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;

namespace OperationBlueholeServer
{
    public class PlayerData
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

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
    }

    public class SimulationResult
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        // 참가한 player id 목록
        [JsonProperty("player")]
        public List<ulong> PlayerList { get; set; }

        // 시뮬레이션에 사용한 random seed 값
        [JsonProperty("seed")]
        public uint Seed { get; set; }

        // 확인 안 한 숫자 - 0되면 자료 삭제
        [JsonProperty("unreadCount")]
        public byte UnreadCount { get; set; }
    }

    public sealed class DbHelper
    {
        private static volatile DbHelper instance;
        private static object syncRoot = new Object();

        private DbHelper() { }

        public static DbHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DbHelper();
                    }
                }

                return instance;
            }
        }

        public static PlayerData GetPlayerData(ulong playerId)
        {
            var client = CouchbaseManager.Instance;
            return client.GetJson<PlayerData>("player" + playerId);
        }

        public static bool SetPlayerData(PlayerData data)
        {
            var client = CouchbaseManager.Instance;
            return client.StoreJson(StoreMode.Set, "player" + data.Id, data);
        }
    }
}