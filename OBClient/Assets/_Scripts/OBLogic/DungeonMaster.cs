using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OperationBluehole.Content
{
    public class DungeonMaster
    {
        // 사실상 게임을 진행하는 로직
        // 맵을 생성하고, 플레이어를 위치 시킨다
        // 플레이어가 행동을 인풋하게 하고 그걸 받아서 내부적으로 적용시킨다
        // 전투가 일어나면 관련 로직을 불러다가 전투 수행하고 결과를 적용
        // 대충 뭐 그런 거 하면 되는 거 아닌가

        // 클라이언트에서 리플레이를 할 때 필요한 정보는 크게 세 가지로 나눌 수 있다
        // 처음 dungeon master가 초기화 되었을 때 복사할 정보 - 시뮬레이션 과정에 영향을 받아서 정보가 바뀔 수 있는 것
        //      map, mobList
        // 시뮬레이션과 상관없이 항상 고정적인 값들
        //      users, itemList
        // 시뮬레이션이 끝나야 알 수 있는 정보 언제든지 필요하면 접근할 수 있는 정보
        //      path, battle log, looted items, looted gold, looted exp

        // users는 어차피 외부에서 입력받는 것이므로 고려하지 않아도 되고,
        // 크게 시뮬레이션 전에 복사해둘 정보와 시뮬레이션 끝나고 참조할 정보로 나눈다
        // 복사할 정보 : map, mobList, itemList
        // 참조할 정보 : path, battle log, looted items, looted gold, looted exp

        // 복사할 정보들은 밖에서 get으로 내부 정보를 접근할 수 있도록 접근 속성을 수정
        // 참조할 정보들은 내부에 컨테이너를 하나 만들어서 저장해둔다
 
        public GameRecord record;

        private Dungeon dungeon;
        private Party users;
        private Explorer explorer;

        // private List<Item> lootedItems;
        // private int lootedGold;
        // private int lootedExp;
        
        public List<Party> mobs;
        public List<Item> items;
        private RandomGenerator random;

        // FOR DEBUG
        private Party TempMobGenerator()
        {
            Mob[] mob = {
							new Mob( MobGenerator.GetMobData(random, MobType.ZombieFatty,10) ), 
							new Mob( MobGenerator.GetMobData(random, MobType.ZombieFatty,10) ), 
							new Mob( MobGenerator.GetMobData(random, MobType.ZombieFatty,10) )
						};
            Party mobs = new Party( PartyType.MOB, 10 );

            foreach ( Mob p in mob )
                mobs.AddCharacter( p );

            return mobs;
        }

        public bool Init( int size, int seed, Party userParty )
        {
			// user 생성
            this.users = userParty;

			// 던전 생성
            // 일단은 빈 리스트들이지만 던전이 생성되고 나면 내부에서 배치된 것들이 안에 등록된다.
            mobs = new List<Party>();
            items = new List<Item>();
            random = new RandomGenerator( seed );

            record = new GameRecord();
            record.lastPosition.x = -1;
            record.lastPosition.y = -1;

            dungeon = new Dungeon( size, mobs, items, users, random, users.partyLevel );
            explorer = new Explorer( this, size );

            explorer.Init(users.position);

            return true;
        }

		public MapObject[,] GetMapInfo() { return dungeon.map; }
		public char[,] GetDungeonMap() { return dungeon.PrintOutMAP(); }

        public uint Start()
        {
            uint turn = 0;

            while ( true )
            {
                ++turn;

                #region 결과 기록
                record.pathfinding.Add(new Int2D { x = explorer.position.x, y = explorer.position.y });
                #endregion

                // 비밀의 방에 도착
                if ( explorer.isRingDiscovered )
                    break;

                MoveDiretion direction = explorer.GetMoveDirection();
                if ( !explorer.Move( direction, record ) )
                    break;

                // 위에서 아이템도 먹고 몹도 처리했으면 실제로 맵에서의 좌표도 이동시킨다
                dungeon.MovePlayer( explorer.position );

                // dungeon.PrintOutMAP();
                // Console.WriteLine( "player position : " + explorer.position.x + " / " + explorer.position.y );

                //Thread.Sleep( 100 );
            }

            // Console.WriteLine( "THE END ( turn : " + turn + " )" );

            // Console.WriteLine( "Earned Exp : " + record.lootedExp );
            // Console.WriteLine( "Earned gold : " + record.lootedGold );
            // Console.WriteLine( "looted items : " );
            // record.lootedItems.ForEach(item => Console.Write(" " + ((ItemToken)item).level));

            return turn;
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
        internal MapObject GetMapObject( int x, int y ) { return dungeon.GetMapObject( x, y ); }

        internal IEnumerable<Item> GetItems( int zoneId )
        {
            return dungeon.zoneList[zoneId].items;
        }

        internal bool StartBattle( Party mob )
        {
            // explorer 좌표에 있는 몹을 읽어와서 전투 시작
            if ( mob.partyType != PartyType.MOB )
                Console.WriteLine( "NOOOOOOOOOOOOOOOO!!" );

            // Console.WriteLine( "Battle : " );
            // Console.ReadLine();

            // 임시 몹 사용
            //Party tempMob = TempMobGenerator();

            // initialize hp, mp, sp
            users.characters.ForEach( each => each.ResetHpMpSp() );

            Battle newBattle = new Battle( random, users, mob );
            #region 전투기록 : 전투기록 설정
            if(record != null)
                newBattle.battleInfo = new BattleInfo(newBattle.party); 
            #endregion
            newBattle.StartBattle();
            #region 전투기록 : 결과 기록
            if (record != null)
                record.battleLog.Add( newBattle.battleInfo.turnInfos );
            #endregion

            if ( newBattle.battleResult == PartyIndex.USERS )
            {
                // 전리품 챙겨라
                mob.characters.ForEach( m =>
                {
                    Mob currentMob = (Mob)m;
                    record.lootedExp += currentMob.rewardExp;
                    record.lootedGold += currentMob.rewardGold;

                    if ( currentMob.rewardItem != null )
                        record.lootedItems.Add( currentMob.rewardItem );
                } );

                // Console.WriteLine( "Test: {0} Win.", (int)newBattle.battleResult );
                // Console.ReadLine();

                return true;
            }

            return false;
        }

        internal void LootItem( Item item, int zoneId )
        {
            // 아이템 줍기!
            // 맵에서도 지우고, 해당 존에서도 지운다
            dungeon.GetMapObject( item.position.x, item.position.y ).gameObject = null;
            dungeon.zoneList[zoneId].items.Remove( item );

            record.lootedItems.Add( item );

            // Console.WriteLine( "looting : " );
            // Console.ReadLine();
        }
    }
}
