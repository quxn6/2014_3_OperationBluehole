﻿using System;
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
            // 전투 로직 초기화
            ContentsPrepare.Init();

            // 던전 테스트--------
            Player[] player = { new Player(), new Player(), new Player() };
            player[0].LoadPlayer( 102 );
            player[1].LoadPlayer( 103 );
            player[2].LoadPlayer( 104 );

            Party users = new Party( PartyType.PLAYER, 10 );
            foreach ( Player p in player )
                users.AddCharacter( p );

			DungeonMaster newMaster = new DungeonMaster();
            newMaster.Init( 60, 4, users );
            Console.WriteLine( "turn : " + newMaster.Start() );
            // ------------------

            Console.ReadLine();
        }
    }

    public static class ContentsPrepare
    {
        public static void Init()
        {
            SkillManager.Init();
            ItemManager.Init();
            MobGenerator.Init();

            TestData.InitPlayer();
        }
    }
}
