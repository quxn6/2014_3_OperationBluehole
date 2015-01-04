using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OperationBluehole.Content
{
    using System.Threading.Tasks;
    using System.Threading;

    class Program
    {
        static Stopwatch time;
        static int testCount = 1000;
        static int finishedTest = 0;
        static int aveTurn = 0;
        static Stack<long> latency = new Stack<long>();

        static void TestSimulation(int seed)
        {
            // 던전 테스트--------
            Player[] player = { new Player(), new Player(), new Player(), new Player() };

            PlayerData data = new PlayerData();
            if ( TestData.playerList.TryGetValue( 102, out data ) )
                player[0].LoadPlayer( data );

            if ( TestData.playerList.TryGetValue( 103, out data ) )
                player[1].LoadPlayer( data );

            if ( TestData.playerList.TryGetValue( 104, out data ) )
                player[2].LoadPlayer( data );

            if ( TestData.playerList.TryGetValue( 101, out data ) )
                player[3].LoadPlayer( data );

            var start = time.ElapsedMilliseconds;

            Party users = new Party( PartyType.PLAYER, 1 );
            foreach ( Player p in player )
                users.AddCharacter( p );

            DungeonMaster newMaster = new DungeonMaster();
            newMaster.Init( 60, seed, users );

            UpdateResult( newMaster.Start(), time.ElapsedMilliseconds - start );

            // 초기 정보 확인
            // var mapInfo = newMaster.GetMapInfo();
            // var itemList = newMaster.items;
            // var mobList = newMaster.mobs;

            // newMaster.TestPathFinding();
            // Console.ReadLine();
            // Debug.WriteLine( "turn : " + newMaster.Start() );

            // 시뮬레이션 결과 확인
            //             foreach( var each in newMaster.record.pathfinding )
            //             {
            //                 Debug.WriteLine( "x : " + each.x + " / y : " + each.y );
            //             }
            // 
            // ------------------
        }

        static void Main(string[] args)
        {
            // 전투 로직 초기화
            ContentsPrepare.Init();

            time = Stopwatch.StartNew();

            for ( int i = 0; i < testCount; ++i )
            {
                Task.Run( () => TestSimulation(i) );
            }

            Console.ReadLine();
        }

        public static void UpdateResult(uint turn, long simulationTime)
        {
            Interlocked.Add( ref aveTurn, (int)turn ); 
            int currentFinished = Interlocked.Add( ref finishedTest, 1 );

            latency.Push( simulationTime );

            if ( currentFinished == testCount )
            {
                var runningTime = time.ElapsedMilliseconds;

                long shortest = latency.Peek();
                long longest = latency.Peek();

                while ( latency.Count != 0 )
                {
                    long current = latency.Pop();

                    shortest = Math.Min( current, shortest );
                    longest = Math.Max( current, longest );
                }

                Console.WriteLine( "[running time : " + runningTime + " ms]" );
                Console.WriteLine( "[Ave. time : " + runningTime / 1000 + " ms]" );
                Console.WriteLine( "[Ave. turn : " + aveTurn / 1000 + " ms]" );
                Console.WriteLine( "[longest : " + longest + " ms]" );
                Console.WriteLine( "[shortest : " + shortest + " ms]" );

            }
        }
    }

    public static class ContentsPrepare
    {
        public static void Init()
        {
            Console.WriteLine( "Initialize contents" );
            SkillManager.Init();
            ItemManager.Init();
            MobGenerator.Init();

            TestData.InitPlayer();
        }
    }
}
