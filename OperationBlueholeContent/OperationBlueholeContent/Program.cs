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
			ItemManager.Init();
			TestData.InitPlayer();
			Player[] player = { new Player(), new Player(), new Player() };
			Mob[] mob = { new Mob(), new Mob(), new Mob() };
			player[0].LoadPlayer(102);
			player[1].LoadPlayer(103);
			player[2].LoadPlayer(104);
			Party Users = new Party(PartyType.PLAYER);
			Party Mobs = new Party(PartyType.MOB);
			foreach (Player p in player)
				Users.AddCharacter(p);
			foreach (Mob p in mob)
				Mobs.AddCharacter(p);
			Random rnd = new Random();
			Battle newBattle = new Battle(rnd, Users, Mobs);
			newBattle.StartBattle();
			// -----------------

            // 던전 테스트--------
            DungeonMaster newMaster = new DungeonMaster();
            newMaster.Init( Users, 60 );
            newMaster.Start();
            // ------------------

			Console.WriteLine("Test: {0} Win.", newBattle.battleResult);
			Console.ReadKey();
        }
    }
}
