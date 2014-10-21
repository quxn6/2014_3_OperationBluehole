using System;
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

    public class GenerationTreeNode
    {
        const int MININUM_SPAN = 12;
        const int SLICE_WEIGHT_1 = 1;
        const int SLICE_WEIGHT_2 = 2;
        const int SILCE_WEIGHT_TOTAL = SLICE_WEIGHT_1 + SLICE_WEIGHT_2;

        public GenerationTreeNode parent, leftChild, rightChild;
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

            leftChild = new GenerationTreeNode();
            rightChild = new GenerationTreeNode();

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
    }

    class Dungeon
    {
        // 컨텐츠 관련 상수들 따로 뺄 것
        const int MAX_OFFSET = 2;
        const float MOB_DENSITY = 0.05f;
        const float ITEM_DENSITY = 0.05f;

        private int size;

        // null 이면 그냥 타일로? 모든 타일의 객체 생성 비용이 부담되면 나중에 수정할 것
        // 일단 구현부터 빨리...
        private MapObject[,] map;

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

        private void GenerateMap( List<Party> mobs, List<Item> items, Party users )
        {
            // 맵 상의 임의의 영역에 대해서
            // 각 영역은 멤버로 자신이 생성된 방향(horizontal, verical)과 영역좌표(AABB)를 가진다
            // 부모 영역에 대한 참조와 트리에서의 depth정보도 가진다.
            // 만약 자신의 depth가 upper bound보다 작으면 아래의 작업을 수행한다
            // 자신의 반대 방향으로 임의의 위치에서 다시 잘라서 해당 영역을 자식으로 추가
            Random random = new Random();

            GenerationTreeNode root = new GenerationTreeNode();
            root.SetRoot( size, random );
            root.GenerateRecursivly();

            // playerParty와 ringOfErr.. 배치용도로 사용
            RingOfErrethAkbe ring = new RingOfErrethAkbe();
            int leafNodeCount = (int)Math.Pow( 2, root.upperDepth );
            bool isPlayerRegistered = false;
            bool isRingRegisteres = false;

            // 완료 후에 root부터 depth가 작은 애들부터 스택에 집어 넣는다
            // 스택에 있는 애들 꺼내면서 leaf인 경우에는 아이템이나 몬스터 배치를 하고
            // 서로 연결한다 - 이걸 표시하는 자료형이 하나 내부에 존재해야 할 듯
            // sibling과 연결할 때 depth가 높은 노드부터 처리하기 위해서 queue 사용
            Stack<GenerationTreeNode> nodes = new Stack<GenerationTreeNode>();
            Queue<GenerationTreeNode> queue = new Queue<GenerationTreeNode>();

            queue.Enqueue( root );

            while ( queue.Count != 0 )
            {
                GenerationTreeNode tempNode = queue.Dequeue();

                nodes.Push( tempNode );

                // 반드시 left - right 순서로 넣을 것
                // 나중에 두 영역을 복도로 연결할 때 영향을 준다!
                // child가 하나인 경우에 대한 예외 처리 필요 - assert!
                if ( tempNode.leftChild != null )
                    queue.Enqueue( tempNode.leftChild );

                if ( tempNode.rightChild != null )
                    queue.Enqueue( tempNode.rightChild );
            }

            int leafNodeIdx = 0;// 1부터 유효한 값
            while( nodes.Count > 0 )
            {
                GenerationTreeNode current = nodes.Pop();

                if ( current.depth == current.upperDepth )
                {
                    // leaf인 경우
                    ++leafNodeIdx; 

                    // 일정 거리를 offset해서 벽을 생성하고
                    // 그 안에 아이템과 몬스터를 배치하자
                    int shortSpan = Math.Min( current.upperBoundary.x - current.lowerBoundary.x, current.upperBoundary.y - current.lowerBoundary.y ) + 1;
                    int offset = Math.Min( random.Next( 0, MAX_OFFSET ), (shortSpan - 3) / 2 );

                    current.lowerBoundary.x += offset;
                    current.upperBoundary.x -= offset;
                    current.lowerBoundary.y += offset;
                    current.upperBoundary.y -= offset;

                    // 벽과 타일 생성
                    for ( int i = current.lowerBoundary.y; i <= current.upperBoundary.y; ++i )
                    {
                        for ( int j = current.lowerBoundary.x; j <= current.upperBoundary.x; ++j )
                        {
                            if ( i == current.lowerBoundary.y || i == current.upperBoundary.y 
                                || j == current.lowerBoundary.x || j == current.upperBoundary.x )
                                map[i, j] = new MapObject( MapObjectType.WALL, null );
                            else
                                map[i, j] = new MapObject( MapObjectType.TILE, null );
                        }
                    }

                    // player와 ring 배치
                    if ( leafNodeIdx <= leafNodeCount / 2 )
                    {
                        if ( !isPlayerRegistered && random.Next(leafNodeIdx, leafNodeCount / 2) == leafNodeCount / 2 )
                        {
                            isPlayerRegistered = true;

                            // player 배치
                            int x = random.Next( current.lowerBoundary.x + 1, current.upperBoundary.x - 1 );
                            int y = random.Next( current.lowerBoundary.y + 1, current.upperBoundary.y - 1 );

                            map[y, x].party = users; // 파티는 어차피 하나만 넣으니까 다른 오브젝트들 확인 안 함
                        }
                    }
                    else 
                    {
                        if ( !isRingRegisteres && random.Next(leafNodeIdx, leafNodeCount) == leafNodeCount )
                        {
                            isRingRegisteres = true;

                            // ring 배치
                            while ( true )
                            {
                                int x = random.Next( current.lowerBoundary.x + 1, current.upperBoundary.x - 1 );
                                int y = random.Next( current.lowerBoundary.y + 1, current.upperBoundary.y - 1 );

                                if ( map[y, x].gameObject == null )
                                {
                                    map[y, x].gameObject = ring;
                                    break;
                                }
                            }
                        }
                    }

                    // 아이템과 몹 배치
                    int tileCount = ( current.upperBoundary.y - current.lowerBoundary.y - 1 ) 
                        * ( current.upperBoundary.x - current.lowerBoundary.x - 1 );

                    int mobCount = (int)( tileCount * MOB_DENSITY );
                    for ( int i = 0; i < mobCount; ++i )
                    {
                        while ( true )
                        {
                            int x = random.Next( current.lowerBoundary.x + 1, current.upperBoundary.x - 1 );
                            int y = random.Next( current.lowerBoundary.y + 1, current.upperBoundary.y - 1 );

                            if ( map[y, x].party == null )
                            {

                                mobs.Add( new Party( PartyType.MOB ) );
                                int idx = mobs.Count - 1;

                                // 조심해!
                                // party에 자신의 idx 기록은 안 해도 되려나...
                                map[y, x].party = mobs[idx];

                                break;
                            }
                        }
                    }
                    
                    int itemCOunt = (int)( tileCount * ITEM_DENSITY );
                    for ( int i = 0; i < itemCOunt; ++i )
                    {
                        while ( true )
                        {
                            int x = random.Next( current.lowerBoundary.x + 1, current.upperBoundary.x - 1 );
                            int y = random.Next( current.lowerBoundary.y + 1, current.upperBoundary.y - 1 );

                            if ( map[y, x].gameObject == null )
                            {
                                items.Add( new Item() );
                                int idx = items.Count - 1;

                                // 조심해!
                                // item에 자신의 idx 기록은 안 해도 되려나...
                                map[y, x].gameObject = items[idx];

                                break;
                            }
                        }
                    }
                }

                // sibling과 연결하자
                // stack 안에는 이미 depth가 높은 노드들이 위에 오도록 들어있으므로 그냥 차례대로 꺼내면서 연결하면 된다
                // 항상 sibling과 쌍으로 들어가고, 순서는 left - right 이므로 
                // 하나 꺼내서 부모를 통해서 sibling(right)를 찾고, 둘이 나누어진 방식(horizontal, vertical)에 따라서 
                // 겹쳐지는 영역을 설정하고, 그 범위 안에서 임의의 idx 선택
                if ( current.isLeft )
                {
                    if ( current.parent == null )
                        continue;

                    GenerationTreeNode siblingNode = current.parent.rightChild;

                    if ( current.siblingDirection )
                    {
                        // y축으로 겹치는 구간 탐색
                        int corridorIdx = random.Next( 
                            Math.Max( current.lowerBoundary.y, siblingNode.lowerBoundary.y ) + 1, 
                            Math.Min( current.upperBoundary.y, siblingNode.upperBoundary.y ) - 1 );

                        LinkHorizontalArea( corridorIdx, current.upperBoundary.x, -1 );
                        LinkHorizontalArea( corridorIdx, current.upperBoundary.x + 1, 1 );
                    }
                    else
                    {
                        // x축으로 겹치는 구간 탐색
                        int corridorIdx = random.Next( 
                            Math.Max( current.lowerBoundary.x, siblingNode.lowerBoundary.x ) + 1, 
                            Math.Min( current.upperBoundary.x, siblingNode.upperBoundary.x ) - 1 );

                        LinkVerticalArea( corridorIdx, current.upperBoundary.y, -1 );
                        LinkVerticalArea( corridorIdx, current.upperBoundary.y + 1, 1 );
                    }
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
