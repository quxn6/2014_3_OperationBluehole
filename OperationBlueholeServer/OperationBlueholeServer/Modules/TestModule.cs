using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Couchbase;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;

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
        }
    }
}