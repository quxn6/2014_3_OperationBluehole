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

        public void Insert(Stack<GenerationTreeNode> stack)
        {
            stack.Push( this );
            
            if ( leftChild != null )
                leftChild.Insert( stack );
            
            if ( rightChild != null )
                rightChild.Insert( stack );
        }
    }

    class Dungeon
    {
        private int size;

        // null 이면 그냥 타일로? 모든 타일의 객체 생성 비용이 부담되면 나중에 수정할 것
        // 일단 구현부터 빨리...
        private GameObject[,] map;  

        public Dungeon( int size )
        {
            this.size = size;
            map = new GameObject[size, size];

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
            Stack<GenerationTreeNode> nodes = new Stack<GenerationTreeNode>();

            root.Insert( nodes );

            // for debug
            int zoneId = 0;
            int[,] testMap = new int[size, size];

            while( nodes.Count > 0 )
            {
                GenerationTreeNode current = nodes.Pop();

                if ( current.depth < current.upperDepth )
                    continue;

                ++zoneId;
                for ( int i = current.lower.y; i <= current.upper.y; ++i )
                {
                    for ( int j = current.lower.x; j <= current.upper.x; ++j )
                    {
                        testMap[i, j] = zoneId;
                    }
                }

                // Console.WriteLine( "lower x : " + current.lower.x + " y : " + current.lower.y );
                // Console.WriteLine( "upper x : " + current.upper.x + " y : " + current.upper.y );
            }

            for ( int i = 0; i < size; ++i )
            {
                for ( int j = 0; j < size; ++j )
                {
                    Console.Write( testMap[i, j] );
                }
                Console.WriteLine( "" );
            }

            /*
            // for debug
            char[] visualizer = { ' ', 'X', 'I', 'M', 'P' };

            for ( int i = 0; i < size; ++i )
            {
                for ( int j = 0; j < size; ++j )
                {
                    Console.Write( visualizer[(int)map[i, j].id] );
                }
                Console.WriteLine("");
            }*/
        }


    }
}
