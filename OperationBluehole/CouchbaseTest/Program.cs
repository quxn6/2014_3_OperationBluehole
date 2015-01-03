using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CouchbaseTest
{
    using OperationBluehole.Database;

    class Program
    {
        const int testDataLen = 516;
        const long testCount = 1000000;
        const int threadNumber = 32;
        static Stopwatch stopWatch;
        static int finishedThread = 0;
        static string testData = "";

        static void Main( string[] args )
        {
            /*
            // 10000000개 데이터 넣고
            Console.WriteLine("generate test data (len=" + testDataLen + ")" );
            for ( int i = 0; i < testDataLen; ++i)
            {
                testData += '0';
            }

            Console.WriteLine( "insert " + testCount + " data" );
            for ( long i = 0; i < testCount; ++i )
            {
                TestDatabase.SetTestData( i + ' ' + testData, i );
                Console.WriteLine(i);
            }
            */
            // 시작 시간 기록
            stopWatch = Stopwatch.StartNew();

            Console.WriteLine( "start!" );
            // 스레드 몇 개 만들어서
            for ( int i = 0; i < threadNumber; ++i )
            {
                Thread workerThread = new Thread( () => Test(i) );
                workerThread.Start();
            }
            Console.ReadLine();
        }

        static void FinishTest()
        {
            int currentFinished = Interlocked.Add( ref finishedThread, 1 );

            if ( currentFinished == threadNumber )
            {
                var runningTime = stopWatch.ElapsedMilliseconds;
                // 모든 작업 완료 시점에 시간 기록
                Console.WriteLine( "[total time : " + runningTime + " ms]" );

                // transaction/min 계산
                Console.WriteLine( "requests / sec : " + 2 * testCount / ( runningTime / 1000.0f ) );

                Console.ReadLine();
            }
        }

        static void Test( int range )
        {
            Console.WriteLine("worker thread start");
            long start = range * ( testCount / threadNumber );
            long end = ( range + 1 ) * ( testCount / threadNumber ) - 1;
            for ( long i = start; i < end; ++i )
            {
                var data = TestDatabase.GetTestData( i );
                TestDatabase.SetTestData( data, i );
            }
        }
    }
}
