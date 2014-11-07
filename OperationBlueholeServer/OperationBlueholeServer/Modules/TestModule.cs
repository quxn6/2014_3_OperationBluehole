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

    public class Beer
    {
        [JsonProperty( "name" )]
        public string Name { get; set; }

        [JsonProperty( "abv" )]
        public float ABV { get; set; }

        [JsonProperty( "type" )]
        public string Type
        {
            get { return "beer"; }
        }

        [JsonProperty( "brewery_id" )]
        public string BreweryId { get; set; }

        [JsonProperty( "style" )]
        public string Style { get; set; }

        [JsonProperty( "category" )]
        public string Category { get; set; }
    }

    public class PlayerData
    {
        [JsonProperty( "player id" )]
        public uint PlayerId { get; set; }

        [JsonProperty( "name" )]
        public string Name { get; set; }

        [JsonProperty( "baseStat" )]
        public ushort[] BaseStat { get; set; }

        [JsonProperty( "Exp" )]
        public uint Exp { get; set; }

        [JsonProperty( "skill list" )]
        public List<ushort> SkillList { get; set; }

        [JsonProperty( "item list" )]
        public List<uint> ItemList { get; set; }

        [JsonProperty( "equipments" )]
        public List<uint> Equipments { get; set; }

        [JsonProperty( "battle style" )]
        public byte BattleStyle { get; set; }
    }

    // 한다! 테스트! 
    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get["/couchbase/"] = parameters =>
            {
                var client = CouchbaseManager.Instance;

                var savedBeer = client.Get( "new_holland_brewing_company-sundog" );

                return savedBeer;
            };
        }
    }
}