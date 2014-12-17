using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.DummyClient
{
    public static class TestResult
    {
        public static int registeredCount = 0;
        public static int updateRequestCount = 0;
        public static int totalUsers = 0;
        public static int finishedUsers = 0;

        public static void PrintResult()
        {
            Console.Clear();
            Console.WriteLine( "Test result :" );
            Console.WriteLine( "Test user number :" + totalUsers );

            Console.WriteLine( "total simulation count : " + registeredCount + " (" + registeredCount/totalUsers + "/each)" );
            Console.WriteLine( "total update request count : " + updateRequestCount + " (" + updateRequestCount/totalUsers + "/each)" );
        }
    }
}
