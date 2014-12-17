using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OperationBluehole.Content
{
    class Program
    {
        static void Main(string[] args)
        {
            // 전투 로직 초기화
            ContentsPrepare.Init();

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

            Party users = new Party( PartyType.PLAYER, 3 );
            foreach ( Player p in player )
                users.AddCharacter( p );

			DungeonMaster newMaster = new DungeonMaster();
            newMaster.Init( 60, 3, users );

            // 초기 정보 확인
            var mapInfo = newMaster.GetMapInfo();
            var itemList = newMaster.items;
            var mobList = newMaster.mobs;

            Debug.WriteLine( "turn : " + newMaster.Start() );

            // 시뮬레이션 결과 확인
//             foreach( var each in newMaster.record.pathfinding )
//             {
//                 Debug.WriteLine( "x : " + each.x + " / y : " + each.y );
//             }
// 
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
