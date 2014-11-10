using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Couchbase;
using Couchbase.Extensions;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;

using OperationBlueholeContent;

namespace OperationBlueholeServer.Modules
{
    using Nancy;

    // 한다! 테스트! 
    public class TestModule : NancyModule
    {
        public TestModule()
        {
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

                var client = CouchbaseManager.Instance;
                var result = client.StoreDictionary(StoreMode.Set, "testPlayer_0", playerData);

                if (result.Item1) 
                    return "success";
                
                return "fail";
            };

            Get["/dict_load"] = parameters =>
            {
                var client = CouchbaseManager.Instance;
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

                return DbHelper.SetPlayerData(testData);
            };

            Get["/load_test_class"] = parameters =>
            {
                var savedData = DbHelper.GetPlayerData( 0 );

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
                DungeonMaster newMaster = new DungeonMaster();
                newMaster.Init(60, 4);

                return "turn : " + newMaster.Start();
            };
        }
    }
}