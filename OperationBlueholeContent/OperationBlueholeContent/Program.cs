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
            DungeonMaster newMaster = new DungeonMaster();
            newMaster.Init( null, 60 );

            Console.WriteLine( "end" );
            Console.ReadLine();
        }
    }
}
