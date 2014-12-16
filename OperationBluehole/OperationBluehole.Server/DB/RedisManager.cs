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

            var result = _redis.GetRangeFromSortedSetDesc( Config.RANKING_SET_NAME, 1, 3 );
            result.ForEach( each => Console.WriteLine( each ) );

            // 기존 값 수정
            _redis.IncrementItemInSortedSet( Config.RANKING_SET_NAME, "two", 9 );

            var fixedResult = _redis.GetRangeFromSortedSetDesc( Config.RANKING_SET_NAME, 1, 3 );
            fixedResult.ForEach( each => Console.WriteLine( each ) );

            // 현재 rank
            var currentRank = _redis.GetItemIndexInSortedSet( Config.RANKING_SET_NAME, "two" );
            Console.WriteLine( "current rank : " + currentRank );

            // 전체 수 구하기
            Console.WriteLine( "total count : " + _redis.GetSortedSetCount( Config.RANKING_SET_NAME ) );
        }

        public static bool RegisterPlayerRank( string playerId )
        {
            return _redis.AddItemToSortedSet( Config.RANKING_SET_NAME, playerId, 0 );
        }

        public static bool UpdateRank(string playerId, long score)
        {
            return score == _redis.IncrementItemInSortedSet( Config.RANKING_SET_NAME, playerId, score );
        }

        public static long GetRank( string playerId )
        {
            return _redis.GetItemIndexInSortedSetDesc( Config.RANKING_SET_NAME, playerId );
        }

        // 자신 점수 주변 사람들 아이디랑 점수 알려줄까...

    }
}
