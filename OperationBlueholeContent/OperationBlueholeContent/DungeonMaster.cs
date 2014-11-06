﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
    class DungeonMaster
    {
        // 사실상 게임을 진행하는 로직
        // 맵을 생성하고, 플레이어를 위치 시킨다
        // 플레이어가 행동을 인풋하게 하고 그걸 받아서 내부적으로 적용시킨다
        // 전투가 일어나면 관련 로직을 불러다가 전투 수행하고 결과를 적용
        // 대충 뭐 그런 거 하면 되는 거 아닌가

        private Dungeon dungeon;
        private Party users;
        private Explorer explorer;
        private List<Item> lootedItems;
        private List<Party> mobs;
        private List<Item> items;
        private RandomGenerator random;

        // HARD CODED
        private Party LoadPlayers()
        {
            TestData.InitPlayer();

            Player[] player = { new Player(), new Player(), new Player() };
            player[0].LoadPlayer( 102 );
            player[1].LoadPlayer( 103 );
            player[2].LoadPlayer( 104 );

            Party users = new Party( PartyType.PLAYER, 10 );
            foreach ( Player p in player )
                users.AddCharacter( p );

            return users;
        }

        // FOR DEBUG
        private Party TempMobGenerator()
        {

            Mob[] mob = { new Mob( 10 ), new Mob( 10 ), new Mob( 10 ) };
            Party mobs = new Party( PartyType.MOB, 10 );

            foreach ( Mob p in mob )
                mobs.AddCharacter( p );

            return mobs;
        }

        public bool Init( int size, int seed )
        {
            // 전투 로직 초기화
            SkillManager.Init();
            ItemManager.Init();

			// user 생성
			this.users = LoadPlayers();

			// 던전 생성
            // 일단은 빈 리스트들이지만 던전이 생성되고 나면 내부에서 배치된 것들이 안에 등록된다.
            mobs = new List<Party>();
            items = new List<Item>();
            lootedItems = new List<Item>();
            random = new RandomGenerator( seed );

            dungeon = new Dungeon( size, mobs, items, users, random, users.partyLevel );
            explorer = new Explorer( this, size );

            explorer.Init( users.position );

            return true;
        }

        public void Start()
        {
            int turn = 0;

            while ( true )
            {
                ++turn;

                // 비밀의 방에 도착
                if ( explorer.isRingDiscovered )
                    break;

                MoveDiretion direction = explorer.GetMoveDirection();
                explorer.Move( direction );

                // 위에서 아이템도 먹고 몹도 처리했으면 실제로 맵에서의 좌표도 이동시킨다
                dungeon.MovePlayer( explorer.position );
                
                // explorer.GetNextZone();
                // explorer.Teleport( explorer.currentDestination );
                // Console.WriteLine( "zone : " + explorer.GetCurrentZoneId() );

                dungeon.PrintOutMAP();
                Console.WriteLine( "player position : " + explorer.position.x + " / " + explorer.position.y );

                //Thread.Sleep( 100 );
            }

            Console.WriteLine( "THE END ( turn : " + turn + " )" );
        }

        // 구현할 것
        // wrappers
        public Int2D GetZonePosition( int id ) { return dungeon.GetZonePosition( id ); }

        public int GetZoneId( Int2D position ) { return dungeon.GetZoneId( position ); }

        public IEnumerable<int> GetLinkedZoneList( int zoneId )
        {
            return dungeon.zoneList[zoneId].linkedZone.Select( z => z.zoneId );
        }

        public bool IsTile( int x, int y ) { return MapObjectType.TILE == dungeon.GetMapObject( x, y ).objectType; }
        public MapObject GetMapObject( int x, int y ) { return dungeon.GetMapObject( x, y ); }

        public IEnumerable<Item> GetItems( int zoneId )
        {
            return dungeon.zoneList[zoneId].items;
        }

        public void StartBattle( Party mob )
        {
            // explorer 좌표에 있는 몹을 읽어와서 전투 시작
            if ( mob.partyType != PartyType.MOB )
                Console.WriteLine( "NOOOOOOOOOOOOOOOO!!" );

            Console.WriteLine( "Battle : " );
            Console.ReadLine();

            // 임시 몹 사용
            Party tempMob = TempMobGenerator();

            Battle newBattle = new Battle( random, users, tempMob );
            newBattle.StartBattle();

            Console.WriteLine( "Test: {0} Win.", newBattle.battleResult );
            Console.ReadLine();
        }

        public void LootItem( Item item, int zoneId )
        {
            // 아이템 줍기!
            // 맵에서도 지우고, 해당 존에서도 지운다
            dungeon.GetMapObject( item.position.x, item.position.y ).gameObject = null;
            dungeon.zoneList[zoneId].items.Remove( item );

            lootedItems.Add( item );

            Console.WriteLine( "looting : " );
            Console.ReadLine();
        }
    }
}
