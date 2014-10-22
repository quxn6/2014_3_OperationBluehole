﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
    public struct Int2D
    {
        public int x, y;

        public Int2D( int x, int y ) { this.x = x; this.y = y; }
    };

    class DungeonTreeNode
    {
        const int MININUM_SPAN = 12;
        const int SLICE_WEIGHT_1 = 1;
        const int SLICE_WEIGHT_2 = 2;
        const int SILCE_WEIGHT_TOTAL = SLICE_WEIGHT_1 + SLICE_WEIGHT_2;

        public DungeonTreeNode parent, leftChild, rightChild;
        public int depth, upperDepth;
        public bool siblingDirection;   // 자신의 sibling zone이 있는 방향 - true이면 수평 방향
        public bool isLeft;             // leftChild인가

        public Int2D lowerBoundary, upperBoundary;      

        public Random random;

        public void SetRoot( int size, Random random )
        {
            this.random = random;

            this.upperBoundary.x = size - 1;
            this.upperBoundary.y = size - 1;
            this.upperDepth = size / MININUM_SPAN;  // 결과가 짝수이면 각각의 축에 대해서 이 결과만큼의 조각으로 나누어짐
            this.siblingDirection = ( random.Next( 0, 1 ) % 2 == 0 );
            this.isLeft = false;
        }

        public void GenerateRecursivly()
        {
            if ( depth >= upperDepth )
                return;

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

            // 실제로 영역 분할
            SliceArea();
            
            // children 단계에서 다시 반복하도록 호출
            leftChild.GenerateRecursivly();
            rightChild.GenerateRecursivly();
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
                    ( SLICE_WEIGHT_1 * upperBoundary.x + SLICE_WEIGHT_2 * lowerBoundary.x ) / SILCE_WEIGHT_TOTAL,
                    ( SLICE_WEIGHT_2 * upperBoundary.x + SLICE_WEIGHT_1 * lowerBoundary.x ) / SILCE_WEIGHT_TOTAL );

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
                    ( SLICE_WEIGHT_1 * upperBoundary.y + SLICE_WEIGHT_2 * lowerBoundary.y ) / SILCE_WEIGHT_TOTAL,
                    ( SLICE_WEIGHT_2 * upperBoundary.y + SLICE_WEIGHT_1 * lowerBoundary.y ) / SILCE_WEIGHT_TOTAL );

                leftChild.lowerBoundary.x = rightChild.lowerBoundary.x = lowerBoundary.x;
                leftChild.upperBoundary.x = rightChild.upperBoundary.x = upperBoundary.x;

                leftChild.lowerBoundary.y = lowerBoundary.y;
                leftChild.upperBoundary.y = smallUpper;

                rightChild.lowerBoundary.y = smallUpper + 1;
                rightChild.upperBoundary.y = upperBoundary.y;
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
    }

    class Dungeon
    {
        // 컨텐츠 관련 상수들 따로 뺄 것
        const int MAX_OFFSET = 2;
        const float MOB_DENSITY = 0.05f;
        const float ITEM_DENSITY = 0.05f;

        private int size;
        private MapObject[,] map;

        private Random random;
        private int leafNodeCount;
        private bool isPlayerRegistered = false;
        private bool isRingRegisteres = false;

        public Dungeon( int size, List<Party> mobs, List<Item> items, Party users )
        {
            this.size = size;
            map = new MapObject[size, size];

            GenerateMap( mobs, items, users );
        }

        private void LinkHorizontalArea( int corridorIdx, int targetIdx, int step )
        {
            while ( map[corridorIdx, targetIdx] == null || map[corridorIdx, targetIdx].objectType != MapObjectType.TILE )
            {
                map[corridorIdx, targetIdx] = new MapObject( MapObjectType.TILE, null );

                bool isLinked = false;

                if ( map[corridorIdx - 1, targetIdx] == null || map[corridorIdx - 1, targetIdx].objectType != MapObjectType.TILE )
                    map[corridorIdx - 1, targetIdx] = new MapObject( MapObjectType.WALL, null );
                else
                    isLinked = true;

                if ( map[corridorIdx + 1, targetIdx] == null || map[corridorIdx + 1, targetIdx].objectType != MapObjectType.TILE )
                    map[corridorIdx + 1, targetIdx] = new MapObject( MapObjectType.WALL, null );
                else
                    isLinked = true;

                // 두 공간이 연결되었다면 탈출
                if ( isLinked )
                    break;

                targetIdx += step;
            }
        }

        private void LinkVerticalArea( int corridorIdx, int targetIdx, int step )
        {
            while ( map[targetIdx, corridorIdx] == null || map[targetIdx, corridorIdx].objectType != MapObjectType.TILE )
            {
                map[targetIdx, corridorIdx] = new MapObject( MapObjectType.TILE, null );

                bool isLinked = false;

                if ( map[targetIdx, corridorIdx - 1] == null || map[targetIdx, corridorIdx - 1].objectType != MapObjectType.TILE )
                    map[targetIdx, corridorIdx - 1] = new MapObject( MapObjectType.WALL, null );
                else
                    isLinked = true;

                if ( map[targetIdx, corridorIdx + 1] == null || map[targetIdx, corridorIdx + 1].objectType != MapObjectType.TILE )
                    map[targetIdx, corridorIdx + 1] = new MapObject( MapObjectType.WALL, null );
                else
                    isLinked = true;

                // 두 공간이 연결되었다면 탈출
                if ( isLinked )
                    break;

                targetIdx += step;
            }
        }

        // LinkHorizontalArea, LinkVerticalArea를 호출해서 두 sibling 관계의 두 node의 영역을 통로로 잇는 함수
        private void LinkArea( DungeonTreeNode currentNode )
        {
            // stack 안에는 이미 depth가 높은 노드들이 위에 오도록 들어있으므로 그냥 차례대로 꺼내면서 연결하면 된다
            // 항상 sibling과 쌍으로 들어가고, 순서는 left - right 이므로 
            // 하나 꺼내서 부모를 통해서 sibling(right)를 찾고, 둘이 나누어진 방식(horizontal, vertical)에 따라서 
            // 겹쳐지는 영역을 설정하고, 그 범위 안에서 임의의 idx 선택

            if ( currentNode.parent == null )
                return;

            DungeonTreeNode siblingNode = currentNode.parent.rightChild;

            if ( currentNode.siblingDirection )
            {
                // y축으로 겹치는 구간 탐색
                int corridorIdx = random.Next(
                    Math.Max( currentNode.lowerBoundary.y, siblingNode.lowerBoundary.y ) + 1,
                    Math.Min( currentNode.upperBoundary.y, siblingNode.upperBoundary.y ) - 1 );

                LinkHorizontalArea( corridorIdx, currentNode.upperBoundary.x, -1 );
                LinkHorizontalArea( corridorIdx, currentNode.upperBoundary.x + 1, 1 );
            }
            else
            {
                // x축으로 겹치는 구간 탐색
                int corridorIdx = random.Next(
                    Math.Max( currentNode.lowerBoundary.x, siblingNode.lowerBoundary.x ) + 1,
                    Math.Min( currentNode.upperBoundary.x, siblingNode.upperBoundary.x ) - 1 );

                LinkVerticalArea( corridorIdx, currentNode.upperBoundary.y, -1 );
                LinkVerticalArea( corridorIdx, currentNode.upperBoundary.y + 1, 1 );
            }
        }

        // currentNode의 영역을 실제 map에 기록 - 타일과 벽을 생성
        private void BakeMap( DungeonTreeNode currentNode )
        {
            // 벽과 타일 생성
            for ( int i = currentNode.lowerBoundary.y; i <= currentNode.upperBoundary.y; ++i )
            {
                for ( int j = currentNode.lowerBoundary.x; j <= currentNode.upperBoundary.x; ++j )
                {
                    if ( i == currentNode.lowerBoundary.y || i == currentNode.upperBoundary.y
                        || j == currentNode.lowerBoundary.x || j == currentNode.upperBoundary.x )
                        map[i, j] = new MapObject( MapObjectType.WALL, null );
                    else
                        map[i, j] = new MapObject( MapObjectType.TILE, null );
                }
            }
        }

        private Int2D RegisterParty( DungeonTreeNode currentNode, Party party )
        {
            Int2D position = new Int2D( -1, -1 );

            while ( true )
            {
                position = currentNode.GetRandomInternalPosition();

                if ( map[position.y, position.x].party == null )
                {
                    // 조심해!
                    // party에 자신의 idx 기록은 안 해도 되려나...
                    map[position.y, position.x].party = party;

                    break;
                }
            }

            return position;
        }

        private Int2D RegisterGameObject( DungeonTreeNode currentNode, GameObject obj )
        {
            Int2D position = new Int2D( -1, -1 );

            while ( true )
            {
                position = currentNode.GetRandomInternalPosition();

                if ( map[position.y, position.x].gameObject == null )
                {
                    // 조심해!
                    // item에 자신의 idx 기록은 안 해도 되려나...
                    map[position.y, position.x].gameObject = obj;

                    break;
                }
            }

            return position;
        }

        private void AllocateObjects( DungeonTreeNode currentNode, int leafNodeIdx, Party users, List<Party> mobs, Item ring, List<Item> items )
        {
            // player와 ring 배치
            if ( leafNodeIdx <= leafNodeCount / 2 )
            {
                if ( !isPlayerRegistered && random.Next( leafNodeIdx, leafNodeCount / 2 ) == leafNodeCount / 2 )
                {
                    isPlayerRegistered = true;

                    // player 배치
                    Int2D palyerPosition = RegisterParty( currentNode, users );
                }
            }
            else
            {
                if ( !isRingRegisteres && random.Next( leafNodeIdx, leafNodeCount ) == leafNodeCount )
                {
                    isRingRegisteres = true;

                    // ring 배치
                    Int2D ringPosition = RegisterGameObject( currentNode, ring );
                }
            }

            // 아이템과 몹 배치
            int tileCount = ( currentNode.upperBoundary.y - currentNode.lowerBoundary.y - 1 )
                * ( currentNode.upperBoundary.x - currentNode.lowerBoundary.x - 1 );

            int mobCount = (int)( tileCount * MOB_DENSITY );
            for ( int i = 0; i < mobCount; ++i )
            {
                // 몹 생성하고
                mobs.Add( new Party( PartyType.MOB ) );
                int idx = mobs.Count - 1;

                // 등록
                Int2D mobPosition = RegisterParty( currentNode, mobs[idx] );
            }

            int itemCOunt = (int)( tileCount * ITEM_DENSITY );
            for ( int i = 0; i < itemCOunt; ++i )
            {
                // 아이템 생성하고
                items.Add( new Item() );
                int idx = items.Count - 1;

                // 등록
                Int2D itemPosition = RegisterGameObject( currentNode, items[idx] );
            }
        }

        // 맵 생성!
        private void GenerateMap( List<Party> mobs, List<Item> items, Party users )
        {
            // 맵 상의 임의의 영역에 대해서
            // 각 영역은 멤버로 자신이 생성된 방향(horizontal, verical)과 영역좌표(AABB)를 가진다
            // 부모 영역에 대한 참조와 트리에서의 depth정보도 가진다.
            // 만약 자신의 depth가 upper bound보다 작으면 아래의 작업을 수행한다
            // 자신의 반대 방향으로 임의의 위치에서 다시 잘라서 해당 영역을 자식으로 추가
            random = new Random();

            DungeonTreeNode root = new DungeonTreeNode();
            root.SetRoot( size, random );
            root.GenerateRecursivly();

            // playerParty와 ringOfErr.. 배치용도로 사용
            RingOfErrethAkbe ring = new RingOfErrethAkbe();
            leafNodeCount = (int)Math.Pow( 2, root.upperDepth );
            isPlayerRegistered = false;
            isRingRegisteres = false;

            // 완료 후에 root부터 depth가 작은 애들부터 스택에 집어 넣는다
            // 스택에 있는 애들 꺼내면서 leaf인 경우에는 아이템이나 몬스터 배치를 하고
            // sibling과 연결할 때 depth가 높은 노드부터 처리하기 위해서 queue 사용
            Stack<DungeonTreeNode> nodes = new Stack<DungeonTreeNode>();
            Queue<DungeonTreeNode> queue = new Queue<DungeonTreeNode>();

            queue.Enqueue( root );

            while ( queue.Count != 0 )
            {
                DungeonTreeNode tempNode = queue.Dequeue();

                nodes.Push( tempNode );

                // 반드시 left - right 순서로 넣을 것
                // 나중에 두 영역을 복도로 연결할 때 영향을 준다!
                // child가 하나인 경우에 대한 예외 처리 필요 - assert!
                if ( tempNode.leftChild != null )
                    queue.Enqueue( tempNode.leftChild );

                if ( tempNode.rightChild != null )
                    queue.Enqueue( tempNode.rightChild );
            }

            int leafNodeIdx = 0;    // 1부터 유효한 값
            while( nodes.Count > 0 )
            {
                DungeonTreeNode current = nodes.Pop();

                // leaf인 경우
                if ( current.depth == current.upperDepth )
                {
                    ++leafNodeIdx; 

                    // 일정 거리를 offset
                    int shortSpan = Math.Min( current.upperBoundary.x - current.lowerBoundary.x, current.upperBoundary.y - current.lowerBoundary.y ) + 1;
                    int offset = Math.Min( random.Next( 0, MAX_OFFSET ), (shortSpan - 3) / 2 );

                    current.Offset( offset );

                    // 실제 맵에다가 타일과 벽을 기록
                    BakeMap( current );

                    // 플레이어, ring, 몹, 아이템들 생성 및 배치 
                    AllocateObjects( current, leafNodeIdx, users, mobs, ring, items );
                }

                if ( current.isLeft )
                {
                    // sibling과 연결하자
                    LinkArea( current );
                }
            }


            // for debug
            char[] visualizer = { ' ', ' ', 'X', 'I', 'M', 'P' };

            for ( int i = 0; i < size; ++i )
            {
                for ( int j = 0; j < size; ++j )
                {
                    if ( map[i,j] == null )
                        Console.Write( visualizer[0] );
                    else
                    {
                        if ( map[i, j].gameObject != null ) // 일단 무조건 아이템
                            Console.Write( visualizer[3] );
                        else if ( map[i, j].party != null )
                        {
                            if ( map[i, j].party.partyType == PartyType.MOB )
                                Console.Write( visualizer[4] );
                            else
                                Console.Write( visualizer[5] );
                        }
                        else
                            Console.Write( visualizer[(int)map[i, j].objectType] );
                    }
                        
                }
                Console.WriteLine("");
            }
        }


    }
}
