using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Couchbase;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;

namespace OperationBlueholeServer
{
    using Nancy;

    public static class CouchbaseManager
    {
        private readonly static CouchbaseClient _instance;

        static CouchbaseManager()
        {
            _instance = new CouchbaseClient();
        }

        public static CouchbaseClient Instance { get { return _instance; } }
    }

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

    public class MatchingModule
    {
    }
}