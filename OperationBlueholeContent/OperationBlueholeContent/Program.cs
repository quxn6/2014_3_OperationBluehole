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
			// 전투 테스트--------
			SkillManager.Init();
			Player[] player = { new Player(), new Player(), new Player() };
			Mob[] mob = { new Mob(), new Mob(), new Mob() };
            Party Users = new Party( PartyType.PLAYER );
            Party Mobs = new Party( PartyType.MOB );
			foreach( Player p in player )
				Users.AddCharacter(p);
			foreach ( Mob p in mob )
				Mobs.AddCharacter(p);
			Battle newBattle = new Battle(Users, Mobs);
			newBattle.StartBattle();
			Console.WriteLine("Test: {0} Win.", newBattle.battleResult);
			// -----------------

            // 던전 테스트--------
            DungeonMaster newMaster = new DungeonMaster();
            newMaster.Init( Users, 60 );
            // ------------------

			Console.ReadKey();
        }
    }
}
