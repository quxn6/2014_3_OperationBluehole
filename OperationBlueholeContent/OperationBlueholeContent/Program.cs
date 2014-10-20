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
            newMaster.Init( null, 40 );

			// 전투 테스트--------
			Player[] player = { new Player(), new Player(), new Player() };
			Mob[] mob = { new Mob(), new Mob(), new Mob() };
			Party Users = new Party();
			Party Mobs = new Party();
			foreach( Player p in player )
				Users.AddCharacter(p);
			foreach ( Mob p in mob )
				Mobs.AddCharacter(p);
			Battle newBattle = new Battle(Users, Mobs);
			newBattle.StartBattle();
			Console.WriteLine("Test: {0} Win.", newBattle.mBattleResult);
			// -----------------

			Console.ReadKey();
        }
    }
}
