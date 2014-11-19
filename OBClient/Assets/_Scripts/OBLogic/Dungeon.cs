using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationBluehole.Content
{
    public struct Int2D
    {
        public int x, y;

        public Int2D( int x, int y ) { this.x = x; this.y = y; }
        public Int2D( Int2D rhs ) { this.x = rhs.x; this.y = rhs.y; }
    }

    internal class DungeonZone
    {
        public readonly int zoneId;
        public readonly Int2D lowerBoundary, upperBoundary, centerPosition;

        public List<Item> items = new List<Item>();
        public List<Party> mobs = new List<Party>();
        public List<DungeonZone> linkedZone = new List<DungeonZone>();

        public DungeonZone( int zoneId, Int2D lowerBoundary, Int2D upperBoundary )
        {
            this.zoneId = zoneId;
            this.lowerBoundary = lowerBoundary;
            this.upperBoundary = upperBoundary;

            centerPosition.x = ( lowerBoundary.x + upperBoundary.x ) / 2;
            centerPosition.y = ( lowerBoundary.y + upperBoundary.y ) / 2;
        }
    }

    internal class DungeonTreeNode
    {
        // 컨텐츠 관련 상수들 따로 뺄 것
        

        public DungeonTreeNode parent, leftChild, rightChild;
        public int depth, upperDepth;
        public bool siblingDirection;   // 자신의 sibling zone이 있는 방향 - true이면 수평 방향
        public bool isLeft;             // leftChild인가

        public Int2D lowerBoundary, upperBoundary;

        private RandomGenerator random;
        private MapObject[,] map;
        private List<Party> mobs;
        private List<Item> items;
        private List<DungeonZone> zoneList;
        private int usersLevel;

        public void SetRoot( int size, RandomGenerator random, MapObject[,] map, List<Party> mobs, List<Item> items, List<DungeonZone> zoneList, int usersLevel )
        {
            this.random = random;
            this.map = map;
            this.mobs = mobs;
            this.items = items;
            this.zoneList = zoneList;
            this.usersLevel = usersLevel;

            this.upperBoundary.x = size - 1;
            this.upperBoundary.y = size - 1;
            this.upperDepth = size / Config.MININUM_SPAN;  // 결과가 짝수이면 각각의 축에 대해서 이 결과만큼의 조각으로 나누어짐
            this.siblingDirection = ( random.Next( 0, 1 ) % 2 == 0 );
            this.isLeft = false;
        }

        public void GenerateRecursivly()
        {
            // Assert(depth <= upperDepth)
            if ( depth == upperDepth )
            {
                // 여기서 DungeonZone을 생성하자
                // offset을 진행하지만 나중에 통로 지역도 포함하기 위해서 zone의 범위는 offset하기 전으러 설정 = 맵 전체를 커버할 수 있어
                DungeonZone newZone = new DungeonZone( zoneList.Count, lowerBoundary, upperBoundary );
                zoneList.Add( newZone );

                // 조심해!
                // 맵 전체를 두번 순회하고 있음
                // zone id 기록
                for ( int i = lowerBoundary.y; i <= upperBoundary.y; ++i )
                    for ( int j = lowerBoundary.x; j <= upperBoundary.x; ++j )
                        map[i, j].zoneId = newZone.zoneId;

                // leafNode이므로 Bake하고
                // 일정 거리를 offset
                int shortSpan = Math.Min( upperBoundary.x - lowerBoundary.x, upperBoundary.y - lowerBoundary.y ) + 1;
                int offset = Math.Min( random.Next( 0, Config.MAX_OFFSET ), ( shortSpan - 3 ) / 2 );

                Offset( offset );

                // 실제 맵에다가 타일과 벽을 기록
                BakeMap();

                // 플레이어, ring, 몹, 아이템들 생성 및 배치 
                AllocateObjects( newZone );
            }
            else
            {
                // leafNode가 아니므로 분할한다
                leftChild = new DungeonTreeNode();
                rightChild = new DungeonTreeNode();

                // children 멤버 변수들 초기화
                leftChild.parent = rightChild.parent = this;

                leftChild.siblingDirection = rightChild.siblingDirection = !siblingDirection;
                leftChild.depth = rightChild.depth = depth + 1;
                leftChild.upperDepth = rightChild.upperDepth = upperDepth;
                leftChild.isLeft = true;
                rightChild.isLeft = false;

                leftChild.random = rightChild.random = random;
                leftChild.map = rightChild.map = map;
                leftChild.mobs = rightChild.mobs = mobs;
                leftChild.items = rightChild.items = items;
                leftChild.zoneList = rightChild.zoneList = zoneList;
                leftChild.usersLevel = rightChild.usersLevel = usersLevel;

                // 실제로 영역 분할
                SliceArea();

                // 조심해!
                // children 단계에서 다시 반복하도록 호출
                // 나중에 task로 처리할까...
                leftChild.GenerateRecursivly();
                rightChild.GenerateRecursivly();

                // 자식 노드들을 연결한다!
                LinkArea();
            }
        }

        // 현재의 영역을 둘로 나눈다
        private void SliceArea()
        {
            int smallUpper = 0;

            if ( !siblingDirection )
            {
                // 조심해!
                // 중복 코드다
                smallUpper = random.Next(
                    ( Config.SLICE_WEIGHT_1 * upperBoundary.x + Config.SLICE_WEIGHT_2 * lowerBoundary.x ) / Config.SILCE_WEIGHT_TOTAL,
                    ( Config.SLICE_WEIGHT_2 * upperBoundary.x + Config.SLICE_WEIGHT_1 * lowerBoundary.x ) / Config.SILCE_WEIGHT_TOTAL );

                leftChild.lowerBoundary.x = lowerBoundary.x;
                leftChild.upperBoundary.x = smallUpper;

                rightChild.lowerBoundary.x = smallUpper + 1;
                rightChild.upperBoundary.x = upperBoundary.x;

                leftChild.lowerBoundary.y = rightChild.lowerBoundary.y = lowerBoundary.y;
                leftChild.upperBoundary.y = rightChild.upperBoundary.y = upperBoundary.y;
            }
            else
            {
                // 조심해!
                // 중복 코드다
                smallUpper = random.Next(
                    ( Config.SLICE_WEIGHT_1 * upperBoundary.y + Config.SLICE_WEIGHT_2 * lowerBoundary.y ) / Config.SILCE_WEIGHT_TOTAL,
                    ( Config.SLICE_WEIGHT_2 * upperBoundary.y + Config.SLICE_WEIGHT_1 * lowerBoundary.y ) / Config.SILCE_WEIGHT_TOTAL );

                leftChild.lowerBoundary.x = rightChild.lowerBoundary.x = lowerBoundary.x;
                leftChild.upperBoundary.x = rightChild.upperBoundary.x = upperBoundary.x;

                leftChild.lowerBoundary.y = lowerBoundary.y;
                leftChild.upperBoundary.y = smallUpper;

                rightChild.lowerBoundary.y = smallUpper + 1;
                rightChild.upperBoundary.y = upperBoundary.y;
            }
        }

        // node에 할당된 영역을 실제 map에 기록 - 타일과 벽을 생성
        private void BakeMap()
        {
            // 벽과 타일 생성
            for ( int i = lowerBoundary.y; i <= upperBoundary.y; ++i )
            {
                for ( int j = lowerBoundary.x; j <= upperBoundary.x; ++j )
                {
                    if ( i == lowerBoundary.y || i == upperBoundary.y
                        || j == lowerBoundary.x || j == upperBoundary.x )
                        map[i, j].objectType = MapObjectType.WALL;
                    else
                        map[i, j].objectType = MapObjectType.TILE;
                }
            }
        }

        // 현재 영역에서 외곽(wall)을 제외한 영역에서 임의의 위치 좌표 반환
        public Int2D GetRandomInternalPosition()
        {
            Int2D returnValue;

            returnValue.x = random.Next( lowerBoundary.x + 1, upperBoundary.x - 1 );
            returnValue.y = random.Next( lowerBoundary.y + 1, upperBoundary.y - 1 );

            return returnValue;
        }

        public void Offset( int offsetValue )
        {
            // 양수일 때 안쪽으로 offset
            lowerBoundary.x += offsetValue;
            upperBoundary.x -= offsetValue;
            lowerBoundary.y += offsetValue;
            upperBoundary.y -= offsetValue;
        }

        public Int2D RegisterParty( Party party, bool isUsers = false )
        {
            Int2D position = new Int2D( -1, -1 );

            while ( true )
            {
                position = GetRandomInternalPosition();

                if ( map[position.y, position.x] != null && map[position.y, position.x].objectType == MapObjectType.TILE
                    && map[position.y, position.x].party == null )
                {
                    // 플레이어가 아이템 위에서 시작하지 않도록 제어
                    if ( map[position.y, position.x].gameObject != null && isUsers )
                        continue;

                    map[position.y, position.x].party = party;
                    party.position = position;

                    break;
                }
            }

            return position;
        }

        public Int2D RegisterGameObject( GameObject obj )
        {
            Int2D position = new Int2D( -1, -1 );

            while ( true )
            {
                position = GetRandomInternalPosition();

                if ( map[position.y, position.x] != null && map[position.y, position.x].objectType == MapObjectType.TILE
                    && map[position.y, position.x].gameObject == null )
                {
                    // 조심해!
                    // item에 자신의 idx 기록은 안 해도 되려나...
                    map[position.y, position.x].gameObject = obj;
                    obj.position = position;

                    break;
                }
            }

            return position;
        }

        private ItemToken GenerateItemToken()
        {
            return new ItemToken(
                    usersLevel + random.Next(
                    (int)( -usersLevel * Config.LEVEL_RANGE ), (int)( usersLevel * Config.LEVEL_RANGE ) ),
                    random );
        }

        private Party GeneratoMobParty()
        {
            Party mobs = new Party( PartyType.MOB, usersLevel );

            // mob 생성해서 추가할 것
            for ( int j = 0; j < Config.MAX_PARTY_MEMBER; ++j )
            {
                // 조심해!
                // 몹타입에 따라서 Mob을 상속받아서 구현한다면
                // 여기서 타입도 결정해서 그에 맞게 생성해주어야 한다
				MobData newMobData = MobGenerator.GetMobData(random, MobType.Spider,
					(ushort)(usersLevel + random.Next(
					(int)(-usersLevel * Config.LEVEL_RANGE), (int)(usersLevel * Config.LEVEL_RANGE)))
					);
                Mob mob = new Mob(newMobData);

                mobs.AddCharacter( mob );
            }

            return mobs;
        }

        private void AllocateObjects( DungeonZone zone )
        {
            // 아이템과 몹 배치
            int tileCount = ( upperBoundary.y - lowerBoundary.y - 1 )
                * ( upperBoundary.x - lowerBoundary.x - 1 );

            int mobCount = (int)( tileCount * Config.MOB_DENSITY );
            for ( int i = 0; i < mobCount; ++i )
            {
                // 몹 생성하고
                mobs.Add( GeneratoMobParty() );

                int idx = mobs.Count - 1;

                zone.mobs.Add( mobs[idx] );

                // 등록
                Int2D mobPosition = RegisterParty( mobs[idx] );
            }

            int itemCOunt = (int)( tileCount * Config.ITEM_DENSITY );
            for ( int i = 0; i < itemCOunt; ++i )
            {
                // 아이템 생성하고
                items.Add( GenerateItemToken() );
                int idx = items.Count - 1;

                zone.items.Add( items[idx] );

                // 등록
                Int2D itemPosition = RegisterGameObject( items[idx] );
            }
        }

        private int LinkHorizontalArea( int corridorIdx, int targetIdx, int step )
        {
            while ( map[corridorIdx, targetIdx].objectType != MapObjectType.TILE )
            {
                map[corridorIdx, targetIdx].objectType = MapObjectType.TILE;

                bool isLinked = false;

                if ( map[corridorIdx - 1, targetIdx].objectType != MapObjectType.TILE )
                    map[corridorIdx - 1, targetIdx].objectType = MapObjectType.WALL;
                else
                    isLinked = true;
                    

                if ( map[corridorIdx + 1, targetIdx].objectType != MapObjectType.TILE )
                    map[corridorIdx + 1, targetIdx].objectType = MapObjectType.WALL;
                else
                    isLinked = true;

                // 두 공간이 연결되었다면 탈출
                if ( isLinked )
                    break;

                targetIdx += step;
            }

            return map[corridorIdx, targetIdx].zoneId;
        }

        private int LinkVerticalArea( int corridorIdx, int targetIdx, int step )
        {
            while ( map[targetIdx, corridorIdx].objectType != MapObjectType.TILE )
            {
                map[targetIdx, corridorIdx].objectType = MapObjectType.TILE;

                bool isLinked = false;

                if ( map[targetIdx, corridorIdx - 1].objectType != MapObjectType.TILE )
                    map[targetIdx, corridorIdx - 1].objectType = MapObjectType.WALL;
                else
                    isLinked = true;

                if ( map[targetIdx, corridorIdx + 1].objectType != MapObjectType.TILE )
                    map[targetIdx, corridorIdx + 1].objectType = MapObjectType.WALL;
                else
                    isLinked = true;

                // 두 공간이 연결되었다면 탈출
                if ( isLinked )
                    break;

                targetIdx += step;
            }

            return map[targetIdx, corridorIdx].zoneId;
        }

        // LinkHorizontalArea, LinkVerticalArea를 호출해서 두 sibling 관계의 두 node의 영역을 통로로 잇는 함수
        private void LinkArea()
        {
            // stack 안에는 이미 depth가 높은 노드들이 위에 오도록 들어있으므로 그냥 차례대로 꺼내면서 연결하면 된다
            // 항상 sibling과 쌍으로 들어가고, 순서는 left - right 이므로 
            // 하나 꺼내서 부모를 통해서 sibling(right)를 찾고, 둘이 나누어진 방식(horizontal, vertical)에 따라서 
            // 겹쳐지는 영역을 설정하고, 그 범위 안에서 임의의 idx 선택

            // 연결할 두 영역 사이의 임의의 점에서 시작해서 각각의 영역이 있는 방향으로 한칸씩 이동하면서 타일을 만날 때까지 통로 생성
            // 이렇게 도달한 타일의 zoneId를 기준으로 각각의 영역은 자신과 연결된 영역의 id를 알 수 있게 된다

            int zoneId1, zoneId2;

            if ( leftChild.siblingDirection )
            {
                // y축으로 겹치는 구간 탐색
                int corridorIdx = random.Next(
                    Math.Max( leftChild.lowerBoundary.y, leftChild.lowerBoundary.y ) + 1,
                    Math.Min( leftChild.upperBoundary.y, leftChild.upperBoundary.y ) - 1 );

                zoneId1 = LinkHorizontalArea( corridorIdx, leftChild.upperBoundary.x, -1 );
                zoneId2 = LinkHorizontalArea( corridorIdx, leftChild.upperBoundary.x + 1, 1 );
            }
            else
            {
                // x축으로 겹치는 구간 탐색
                int corridorIdx = random.Next(
                    Math.Max( leftChild.lowerBoundary.x, leftChild.lowerBoundary.x ) + 1,
                    Math.Min( leftChild.upperBoundary.x, leftChild.upperBoundary.x ) - 1 );

                zoneId1 = LinkVerticalArea( corridorIdx, leftChild.upperBoundary.y, -1 );
                zoneId2 = LinkVerticalArea( corridorIdx, leftChild.upperBoundary.y + 1, 1 );
            }

            zoneList[zoneId1].linkedZone.Add( zoneList[zoneId2] );
            zoneList[zoneId2].linkedZone.Add( zoneList[zoneId1] );
        }
    }

    class Dungeon
    {
        public MapObject[,] map;

        private int size;
        private RandomGenerator random;
        private int userLevel;

        private Int2D playerPosition, ringPosition;

        public List<DungeonZone> zoneList = new List<DungeonZone>();

        public Dungeon( int size, List<Party> mobs, List<Item> items, Party users, RandomGenerator random, int userLevel )
        {
            this.size = size;
            map = new MapObject[size, size];
            this.random = random;
            this.userLevel = userLevel;

            // 나중에는 object pool 만들 것
            for ( int i = 0; i < size; ++i )
                for ( int j = 0; j < size; ++j )
                    map[i, j] = new MapObject();

                GenerateMap( mobs, items, users );
        }

        // 맵 생성!
        private void GenerateMap( List<Party> mobs, List<Item> items, Party users )
        {
            // 맵 상의 임의의 영역에 대해서
            // 각 영역은 멤버로 자신이 생성된 방향(horizontal, verical)과 영역좌표(AABB)를 가진다
            // 부모 영역에 대한 참조와 트리에서의 depth정보도 가진다.
            // 만약 자신의 depth가 upper bound보다 작으면 아래의 작업을 수행한다
            // 자신의 반대 방향으로 임의의 위치에서 다시 잘라서 해당 영역을 자식으로 추가
            DungeonTreeNode root = new DungeonTreeNode();
            root.SetRoot( size, random, map, mobs, items, zoneList, userLevel );
            root.GenerateRecursivly();

            // player party와 ring 배치 - 맵 전체를 기반으로
            RingOfErrethAkbe ring = new RingOfErrethAkbe();

            playerPosition = root.RegisterParty( users, true );
            ringPosition = root.RegisterGameObject( ring );
            items.Add(ring);

            zoneList[map[ringPosition.y, ringPosition.x].zoneId].items.Add( ring );

            PrintOutMAP();

            Console.WriteLine( "distance between player and ring" );
            Console.WriteLine( " :" + Math.Abs( playerPosition.x - ringPosition.x ) + " / "+ Math.Abs( playerPosition.y - ringPosition.y ) );
        }

        public bool FindRing( int zoneId )
        {
            // 인자로 받은 존에 ring이 있는 지 확인
            return zoneList[zoneId].items.Where( i => i.code == ItemCode.Ring ).Count() > 0;
        }

        public int GetZoneId( Int2D position )
        {
            return zoneList
                .Where( z => z.lowerBoundary.x <= position.x && position.x <= z.upperBoundary.x && z.lowerBoundary.y <= position.y && position.y <= z.upperBoundary.y )
                .Select( z => z.zoneId )
                .First();
        }

        public Int2D GetZonePosition( int id ) { return zoneList[id].centerPosition; }

        public MapObject GetMapObject( int x, int y ) { return map[y, x]; }

        #region FOR DEBUG
        public char[] PrintOutMAP()
        {
            char[] visualizer = { '#', ' ', 'X', 'I', 'M', 'P' };

            //Console.Clear();
			char[] dungeonMap = new char[size * size];
            for ( int i = 0; i < size; ++i )
            {
                for ( int j = 0; j < size; ++j )
                {
                    if ( j == ringPosition.x && i == ringPosition.y )
                    {
                        Console.Write( 'O' );
						dungeonMap[size * i + j] = 'O';
                        continue;
                    }

                    switch ( map[i, j].objectType )
                    {
                        case MapObjectType.VOID:
                            Console.Write( visualizer[0] );
							dungeonMap[size * i + j] = visualizer[0];
                            break;
                        case MapObjectType.WALL:
                            Console.Write( visualizer[2] );
							dungeonMap[size * i + j] = visualizer[2];
                            break;
                        default:
                            if ( map[i, j].party != null )
                            {
								if ( map[i , j].party.partyType == PartyType.MOB )
								{
									Console.Write( visualizer[4] );
									dungeonMap[size * i + j] = visualizer[4];
								}
								else
								{
									Console.Write( visualizer[5] );
									dungeonMap[size * i + j] = visualizer[5];		
								}
                            }
                            else if ( map[i, j].gameObject != null )
							{
								Console.Write( visualizer[3] );
								dungeonMap[size * i + j] = visualizer[3];		
							}                                
                            else
							{
								Console.Write( visualizer[1] );
								dungeonMap[size * i + j] = visualizer[1];		
							}
                                
                            break;
                    }
                }
                Console.WriteLine( "" );
            }

			return dungeonMap;
        }

        public bool MovePlayer( Int2D position )
        {
            Int2D newPosition = position;

            // 임시 사용 중
            // 해당 영역에 몬스터 있으면 덮어 쓰지 말고 다르게 처리할 것
            // 전투를 하든가...뭐 그런 식으로
            /*
            while ( map[position.y, position.x].party != null )
            {
                Int2D tempPosition;
                tempPosition.x = newPosition.x + random.Next(-3, 3);
                tempPosition.y = newPosition.y + random.Next(-3, 3);

                if ( map[position.y, position.x].objectType != MapObjectType.TILE || map[position.y, position.x].party != null )
                    continue;

                newPosition = tempPosition;
            }
            */
            map[newPosition.y, newPosition.x].party = map[playerPosition.y, playerPosition.x].party;
            map[playerPosition.y, playerPosition.x].party = null;
            playerPosition = newPosition;
            
            return true;
        }
        #endregion
    }
}
