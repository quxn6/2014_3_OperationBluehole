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
        public bool direction, side;    // true : horizontal, closer to origin point

        public Int2D lower, upper;

        public Random random;

        public void SetRoot( int size )
        {
            this.random = new Random();

            this.upper.x = size - 1;
            this.upper.y = size - 1;
            this.upperDepth = size / MININUM_SPAN;  // 결과가 짝수이면 각각의 축에 대해서 이 결과만큼의 조각으로 나누어짐
            this.direction = ( random.Next( 0, 1 ) % 2 == 0 );
        }

        public void GenerateRecursivly()
        {
            if ( depth >= upperDepth )
                return;

            int smallUpper;
            
            leftChild = new GenerationTreeNode();
            rightChild = new GenerationTreeNode();

            leftChild.parent = rightChild.parent = this;

            leftChild.direction = rightChild.direction = !direction;
            leftChild.depth = rightChild.depth = depth + 1;
            leftChild.upperDepth = rightChild.upperDepth = upperDepth;
            leftChild.side = true;
            rightChild.side = false;

            leftChild.random = rightChild.random = random;


            if ( direction )
            {
                // 조심해!
                // 일단 하드코딩
                smallUpper = random.Next( ( upper.x + 2 * lower.x ) / 3, ( 2 * upper.x + lower.x ) / 3 );

                leftChild.lower.x = lower.x;
                leftChild.upper.x = smallUpper;

                rightChild.lower.x = smallUpper + 1;
                rightChild.upper.x = upper.x;

                leftChild.lower.y = rightChild.lower.y = lower.y;
                leftChild.upper.y = rightChild.upper.y = upper.y;
            }
            else
            {
                // 조심해!
                // 중복 코드다
                smallUpper = random.Next( ( upper.y + 2 * lower.y ) / 3, ( 2 * upper.y + lower.y ) / 3 );

                leftChild.lower.x = rightChild.lower.x = lower.x;
                leftChild.upper.x = rightChild.upper.x = upper.x;

                leftChild.lower.y = lower.y;
                leftChild.upper.y = smallUpper;

                rightChild.lower.y = smallUpper + 1;
                rightChild.upper.y = upper.y;
            }

            leftChild.GenerateRecursivly();
            rightChild.GenerateRecursivly();
        }
    }

    class Dungeon
    {
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
                if ( tempNode.leftChild != null )
                    queue.Enqueue( tempNode.leftChild );

                if ( tempNode.rightChild != null )
                    queue.Enqueue( tempNode.rightChild );
            }

            // for debug
            int zoneId = 0;
            bool link = true;
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
                    current.lower.x += offset;
                    current.upper.x -= offset;
                    current.lower.y += offset;
                    current.upper.y -= offset;

                    for ( int i = current.lower.y; i <= current.upper.y; ++i )
                    {
                        for ( int j = current.lower.x; j <= current.upper.x; ++j )
                        {
                            if ( i == current.lower.y || i == current.upper.y || j == current.lower.x || j == current.upper.x )
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
                if ( link )
                {
                    if ( current.parent == null )
                        continue;

                    GenerationTreeNode siblingNode = current.parent.rightChild;

                    if ( current.direction )
                    {
                        // y축으로 겹치는 구간 탐색

                        int corridorIdx = current.random.Next( Math.Min(current.lower.y, siblingNode.lower.y), Math.Max(current.upper.y, siblingNode.upper.y) );

                        // 연결 통로를 뚫자
                    }
                    else
                    {
                        // x축으로 겹치는 구간 탐색

                        int corridorIdx = current.random.Next( Math.Min( current.lower.x, siblingNode.lower.x ), Math.Max( current.upper.x, siblingNode.upper.x ) );

                        // 연결 통로를 뚫어
                    }
                }

                link = !link;

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
            char[] visualizer = { ' ', '*', 'X', 'I', 'M', 'P' };

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
