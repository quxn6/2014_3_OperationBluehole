using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
    class Program
    {
        static void Main(string[] args)
        {
            // 던전 테스트--------
            DungeonMaster newMaster = new DungeonMaster();
            newMaster.Init( 60, 2 );
            newMaster.Start();
            // ------------------

            Console.ReadLine();
        }
    }
}
