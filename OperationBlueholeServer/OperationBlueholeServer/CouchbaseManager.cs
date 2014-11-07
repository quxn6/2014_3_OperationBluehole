using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Couchbase;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;

namespace OperationBlueholeServer
{
    public static class CouchbaseManager
    {
        private readonly static CouchbaseClient _instance;

        static CouchbaseManager()
        {
            _instance = new CouchbaseClient();
        }

        public static CouchbaseClient Instance { get { return _instance; } }
    }
}