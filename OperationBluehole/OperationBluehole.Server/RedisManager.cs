using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Server
{
    using ServiceStack.Redis;
    using OperationBluehole.Content;

    public static class RedisManager
    {
        private readonly static RedisClient _redis;

        static RedisManager()
        {
            _redis = new RedisClient( Config.REDIS_DATABASE_ADDRESS );
        }

        public static void Test()
        {
            _redis.AddItemToSortedSet( Config.RANKING_SET_NAME, "one", 1 );
            _redis.AddItemToSortedSet( Config.RANKING_SET_NAME, "two", 2 );
            _redis.AddItemToSortedSet( Config.RANKING_SET_NAME, "three", 3 );
            _redis.AddItemToSortedSet( Config.RANKING_SET_NAME, "four", 4 );
            _redis.AddItemToSortedSet( Config.RANKING_SET_NAME, "five", 5 );

            var result = _redis.GetRangeFromSortedSet( Config.RANKING_SET_NAME, 1, 3 );
            result.ForEach( each => Console.WriteLine( each ) );

            _redis.IncrementItemInSortedSet( Config.RANKING_SET_NAME, "two", 9 );

            var fixedResult = _redis.GetRangeFromSortedSet( Config.RANKING_SET_NAME, 1, 3 );
            fixedResult.ForEach( each => Console.WriteLine( each ) );

            var currentRank = _redis.GetItemIndexInSortedSet( Config.RANKING_SET_NAME, "two" );
            Console.WriteLine( "current rank : " + currentRank );

            // 기존 값 수정

            // 전체 수 구하기

        }

        public static bool RegisterPlayerRank( PlayerData data )
        {
            long totalExp = 0;
            return _redis.AddItemToSortedSet( Config.RANKING_SET_NAME, data.pId, totalExp );
        }

        public static bool UpdateRank( PlayerData data )
        {
            long totalExp = 0;
            return totalExp == _redis.IncrementItemInSortedSet( Config.RANKING_SET_NAME, data.pId, totalExp );
        }
    }
}
