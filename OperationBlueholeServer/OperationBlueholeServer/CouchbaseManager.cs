using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Dynamic;

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


        // 2. Dynamic type을 활용하는 것
        // 일단 보류
        // Dynamic type 공부한 후에 사용하자

        public static Tuple<bool, int, string> StoreDynamic( this ICouchbaseClient client, StoreMode storeMode, string key, ExpandoObject obj )
        {
            var json = JsonConvert.SerializeObject( obj );
            var result = client.ExecuteStore( storeMode, key, json );

            if ( !result.Success )
            {
                if ( result.Exception != null ) throw result.Exception as Exception;

                return Tuple.Create( false, result.StatusCode.HasValue ? result.StatusCode.Value : -1, result.Message );
            }

            return Tuple.Create( true, 0, string.Empty );
        }

        public static Tuple<bool, int, string, ExpandoObject> GetDynamic( this ICouchbaseClient client, string key )
        {
            var result = client.ExecuteGet<string>( key );

            if ( !result.Success )
            {
                if ( result.Exception != null ) throw result.Exception;

                return Tuple.Create<bool, int, string, ExpandoObject>
                            ( false, result.StatusCode.HasValue ? result.StatusCode.Value : -1, result.Message, null );
            }

            var obj = JsonConvert.DeserializeObject<ExpandoObject>( result.Value );
            return Tuple.Create( true, 0, string.Empty, obj );
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
    }


}