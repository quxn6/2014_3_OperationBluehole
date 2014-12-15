using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Server
{
    using ServiceStack.Redis;

    public static class RedisManager
    {
        static RedisManager()
        {
        }

        public static void Test()
        {
            var redisClient = new RedisClient( "localhost" );

            redisClient.AddItemToSortedSet( "ranking", "one", 1 );
            redisClient.AddItemToSortedSet( "ranking", "two", 2 );
            redisClient.AddItemToSortedSet( "ranking", "three", 3 );

            var result = redisClient.GetRangeFromSortedSet( "ranking", 1, 2 );
            result.ForEach( each => Console.WriteLine( each ) );

            // 기존 값 수정

            // 전체 수 구하기

        }
    }
}
