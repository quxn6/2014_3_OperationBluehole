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
        const int MININUM_SPAN = 10;

        public GenerationTreeNode parent, leftChild, rightChild;
        public int depth, upperDepth;
        public bool siblingDirection;   // 자신의 sibling zone이 있는 방향 - true이면 수평 방향
        public bool isLeft;             // leftChild인가

        public Int2D lowerBoundary, upperBoundary;      

        public Random random;

        public void SetRoot( int size )
        {
            this.random = new Random();

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

            int smallUpper;
            
            leftChild = new GenerationTreeNode();
            rightChild = new GenerationTreeNode();

            leftChild.parent = rightChild.parent = this;

            leftChild.siblingDirection = rightChild.siblingDirection = !siblingDirection;
            leftChild.depth = rightChild.depth = depth + 1;
            leftChild.upperDepth = rightChild.upperDepth = upperDepth;
            leftChild.isLeft = true;
            rightChild.isLeft = false;

            leftChild.random = rightChild.random = random;


            if ( !siblingDirection )
            {
                // 조심해!
                // 일단 하드코딩
                smallUpper = random.Next( ( upperBoundary.x + 2 * lowerBoundary.x ) / 3, ( 2 * upperBoundary.x + lowerBoundary.x ) / 3 );

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
                smallUpper = random.Next( ( upperBoundary.y + 2 * lowerBoundary.y ) / 3, ( 2 * upperBoundary.y + lowerBoundary.y ) / 3 );

                leftChild.lowerBoundary.x = rightChild.lowerBoundary.x = lowerBoundary.x;
                leftChild.upperBoundary.x = rightChild.upperBoundary.x = upperBoundary.x;

                leftChild.lowerBoundary.y = lowerBoundary.y;
                leftChild.upperBoundary.y = smallUpper;

                rightChild.lowerBoundary.y = smallUpper + 1;
                rightChild.upperBoundary.y = upperBoundary.y;
            }

            leftChild.GenerateRecursivly();
            rightChild.GenerateRecursivly();
        }
    }

    class Dungeon
    {
        // 컨텐츠 관련 상수들 따로 뺄 것
        const int MAX_OFFSET = 2;
        private int size;

        // null 이면 그냥 타일로? 모든 타일의 객체 생성 비용이 부담되면 나중에 수정할 것
        // 일단 구현부터 빨리...
        private MapObject[,] map;  

        public Dungeon( int size )
        {
            this.size = size;
            map = new MapObject[size, size];

            GenerateMap();
        }

        private void GenerateMap()
        {
            // 맵 상의 임의의 영역에 대해서
            // 각 영역은 멤버로 자신이 생성된 방향(horizontal, verical)과 영역좌표(AABB)를 가진다
            // 부모 영역에 대한 참조와 트리에서의 depth정보도 가진다.
            // 만약 자신의 depth가 upper bound보다 작으면 아래의 작업을 수행한다
            // 자신의 반대 방향으로 임의의 위치에서 다시 잘라서 해당 영역을 자식으로 추가
            GenerationTreeNode root = new GenerationTreeNode();
            root.SetRoot( size );
            root.GenerateRecursivly();

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

            // for debug
            int zoneId = 0;
            int[,] testMap = new int[size, size];

            while( nodes.Count > 0 )
            {
                GenerationTreeNode current = nodes.Pop();

                if ( current.depth == current.upperDepth )
                {
                    // leaf인 경우
                    // 일정 거리를 offset해서 벽을 생성하고
                    // 그 안에 아이템과 몬스터를 배치하자
                    ++zoneId;

                    int offset = current.random.Next( 0, MAX_OFFSET );
                    current.lowerBoundary.x += offset;
                    current.upperBoundary.x -= offset;
                    current.lowerBoundary.y += offset;
                    current.upperBoundary.y -= offset;

                    for ( int i = current.lowerBoundary.y; i <= current.upperBoundary.y; ++i )
                    {
                        for ( int j = current.lowerBoundary.x; j <= current.upperBoundary.x; ++j )
                        {
                            if ( i == current.lowerBoundary.y || i == current.upperBoundary.y || j == current.lowerBoundary.x || j == current.upperBoundary.x )
                            {
                                map[i, j] = new MapObject( MapObjectId.WALL, null );
                            }
                            else
                            {
                                map[i, j] = new MapObject( MapObjectId.TILE, null );
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
                        int corridorIdx = current.random.Next( Math.Max( current.lowerBoundary.y, siblingNode.lowerBoundary.y ) + 1, Math.Min( current.upperBoundary.y, siblingNode.upperBoundary.y ) - 1 );

                        // 연결 통로를 뚫자
                        // wall이 있는 idx를 선택하면? 망한다
                        // wall을 피해서 만들어야 되는데
                        int targetIdx = current.upperBoundary.x;
                        while ( map[corridorIdx, targetIdx] == null || map[corridorIdx, targetIdx].objectId != MapObjectId.TILE )
                        {
                            map[corridorIdx, targetIdx] = new MapObject( MapObjectId.TILE, null );

                            bool tempTest = false;

                            if ( map[corridorIdx - 1, targetIdx] == null || map[corridorIdx - 1, targetIdx].objectId != MapObjectId.TILE )
                                map[corridorIdx - 1, targetIdx] = new MapObject( MapObjectId.WALL, null );
                            else
                                tempTest = true;

                            if ( map[corridorIdx + 1, targetIdx] == null || map[corridorIdx + 1, targetIdx].objectId != MapObjectId.TILE )
                                map[corridorIdx + 1, targetIdx] = new MapObject( MapObjectId.WALL, null );
                            else
                                tempTest = true;

                            // 둘 중에 하나라도 타일이면 반대편 벽으로 만들고 루프 탈출해야 하는데
                            if ( tempTest )
                                break;

                            --targetIdx;
                        }

                        targetIdx = current.upperBoundary.x + 1;
                        while ( map[corridorIdx, targetIdx] == null || map[corridorIdx, targetIdx].objectId != MapObjectId.TILE )
                        {
                            map[corridorIdx, targetIdx] = new MapObject( MapObjectId.TILE, null );

                            bool tempTest = false;

                            if ( map[corridorIdx - 1, targetIdx] == null || map[corridorIdx - 1, targetIdx].objectId != MapObjectId.TILE )
                                map[corridorIdx - 1, targetIdx] = new MapObject( MapObjectId.WALL, null );
                            else
                                tempTest = true;

                            if ( map[corridorIdx + 1, targetIdx] == null || map[corridorIdx + 1, targetIdx].objectId != MapObjectId.TILE )
                                map[corridorIdx + 1, targetIdx] = new MapObject( MapObjectId.WALL, null );
                            else
                                tempTest = true;

                            // 둘 중에 하나라도 타일이면 반대편 벽으로 만들고 루프 탈출해야 하는데
                            if ( tempTest )
                                break;

                            ++targetIdx;
                        }
                    }
                    else
                    {
                        // x축으로 겹치는 구간 탐색
                        int corridorIdx = current.random.Next( Math.Max( current.lowerBoundary.x, siblingNode.lowerBoundary.x ) + 1, Math.Min( current.upperBoundary.x, siblingNode.upperBoundary.x ) - 1 );

                        // 연결 통로를 뚫자
                        int targetIdx = current.upperBoundary.y;
                        while ( map[targetIdx, corridorIdx] == null || map[targetIdx, corridorIdx].objectId != MapObjectId.TILE )
                        {
                            map[targetIdx, corridorIdx] = new MapObject( MapObjectId.TILE, null );

                            bool tempTest = false;

                            if ( map[targetIdx, corridorIdx - 1] == null || map[targetIdx, corridorIdx - 1].objectId != MapObjectId.TILE )
                                map[targetIdx, corridorIdx - 1] = new MapObject( MapObjectId.WALL, null );
                            else
                                tempTest = true;

                            if ( map[targetIdx, corridorIdx + 1] == null || map[targetIdx, corridorIdx + 1].objectId != MapObjectId.TILE )
                                map[targetIdx, corridorIdx + 1] = new MapObject( MapObjectId.WALL, null );
                            else
                                tempTest = true;

                            // 둘 중에 하나라도 타일이면 반대편 벽으로 만들고 루프 탈출해야 하는데
                            if ( tempTest )
                                break;

                            --targetIdx;
                        }

                        targetIdx = current.upperBoundary.y + 1;
                        while ( map[targetIdx, corridorIdx] == null || map[targetIdx, corridorIdx].objectId != MapObjectId.TILE )
                        {
                            map[targetIdx, corridorIdx] = new MapObject( MapObjectId.TILE, null );

                            bool tempTest = false;

                            if ( map[targetIdx, corridorIdx - 1] == null || map[targetIdx, corridorIdx - 1].objectId != MapObjectId.TILE )
                                map[targetIdx, corridorIdx - 1] = new MapObject( MapObjectId.WALL, null );
                            else
                                tempTest = true;

                            if ( map[targetIdx, corridorIdx + 1] == null || map[targetIdx, corridorIdx + 1].objectId != MapObjectId.TILE )
                                map[targetIdx, corridorIdx + 1] = new MapObject( MapObjectId.WALL, null );
                            else
                                tempTest = true;

                            // 둘 중에 하나라도 타일이면 반대편 벽으로 만들고 루프 탈출해야 하는데
                            if ( tempTest )
                                break;

                            ++targetIdx;
                        }
                    }
                }
                // Console.WriteLine( "lower x : " + current.lower.x + " y : " + current.lower.y );
                // Console.WriteLine( "upper x : " + current.upper.x + " y : " + current.upper.y );
            }
            /*
            for ( int i = 0; i < size; ++i )
            {
                for ( int j = 0; j < size; ++j )
                {
                    Console.Write( testMap[i, j] );
                }
                Console.WriteLine( "" );
            }
            */
            
            // for debug
            char[] visualizer = { ' ', ' ', 'X', 'I', 'M', 'P' };

            for ( int i = 0; i < size; ++i )
            {
                for ( int j = 0; j < size; ++j )
                {
                    if ( map[i,j] == null )
                        Console.Write( visualizer[0] );
                    else
                        Console.Write( visualizer[(int)map[i, j].objectId] );
                }
                Console.WriteLine("");
            }
        }


    }
}
