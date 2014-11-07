using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
    enum MoveDiretion
    {
        STAY,
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }

    internal class ExploerNode : IComparable<ExploerNode>
    {
        public bool isClosed;
        public bool isOpened;
        public ExploerNode cameFrom;
        public float fScore;
        public float gScore;

        public readonly bool isTile;
        public readonly Int2D position;

        public ExploerNode( bool isTile, int x, int y )
        {
            this.isTile = isTile;
            this.position.x = x;
            this.position.y = y;

            isClosed = !isTile;
            isOpened = false;
            cameFrom = null;
            fScore = 0.0f;
            gScore = 0.0f;
        }

        public void Reset()
        {
            if ( isTile )
            {
                isClosed = false;
                isOpened = false;
                cameFrom = null;
                fScore = 0.0f;
                gScore = 0.0f;
            }
        }

        public int CompareTo( ExploerNode obj )
        {
            return this.fScore.CompareTo( obj.fScore );
        }
    }

    // 시야 개념은 던전 내부 zone으로 구분
    // zone 정보는 DM를 통해서 얻음
    class Explorer
    {
        public Int2D position { get; set; }
        public Int2D currentDestination { get; private set; }
        public int currentDestinationId { get; private set; }
        public int currentZoneId { get; private set; }

        private Stack<Int2D> currentMovePath = new Stack<Int2D>();

        private Stack<int>      dungeonZoneHistory = new Stack<int>();  // zone 단위의 기록 - 현재 위치와는 다름
        private HashSet<int> exploredZone = new HashSet<int>();
        private ExploerNode[,] map;
        private int mapSize;
        public bool isRingDiscovered { get; private set; }

        private DungeonMaster dungeonMaster;

        public Explorer( DungeonMaster master, int size )
        {
            this.dungeonMaster = master;
            map = new ExploerNode[size, size];
            mapSize = size;

            for ( int i = 0; i < size; ++i )
                for ( int j = 0; j < size; ++j )
                    map[i, j] = new ExploerNode( dungeonMaster.IsTile( j, i ), j, i );
        }

        public void Init( Int2D position )
        {
            isRingDiscovered = false;
            this.position = position;

            // 존 방문 기록도 업데이트하고, 첫 movePath 계산도 해둔다
            currentZoneId = dungeonMaster.GetZoneId( position );
            dungeonZoneHistory.Push( currentZoneId );
            exploredZone.Add( currentZoneId );

            UpdateDestination();
        }

        // 조심해!
        // FOR DEBUG!!!!
        public void Teleport( Int2D destination )
        {
            position = destination;

            // 현재 존 정보 업데이트 할 것
            int currentZoneId = dungeonMaster.GetZoneId( position );
            if ( currentZoneId != dungeonZoneHistory.Peek() )
            {
                dungeonZoneHistory.Push( currentZoneId );
                exploredZone.Add( currentZoneId );
            }
        }

        // 조심해!
        // FOR DEBUG!!!!
        public void GetNextZone()
        {
            UpdateDestination();
        }

        public MoveDiretion GetMoveDirection()
        {
            if ( currentMovePath.Count == 0 )
                UpdateDestination(); 

            // 비교문 없애려면 2차원 테이블 하나 만들 것
            // move horizontally first.
            if ( position.x > currentMovePath.Peek().x )
                return MoveDiretion.LEFT;
            else if ( position.x < currentMovePath.Peek().x )
                return MoveDiretion.RIGHT;

            if ( position.y > currentMovePath.Peek().y )
                return MoveDiretion.UP;
            else if ( position.y < currentMovePath.Peek().y )
                return MoveDiretion.DOWN;

            // NO WAY!
            return MoveDiretion.STAY;
        }

        public void Move( MoveDiretion direction )
        {
            switch ( direction )
            {
                case MoveDiretion.DOWN:
                    position = new Int2D(position.x, position.y + 1);
                    break;
                case MoveDiretion.LEFT:
                    position = new Int2D( position.x - 1, position.y );
                    break;
                case MoveDiretion.RIGHT:
                    position = new Int2D( position.x + 1, position.y );
                    break;
                case MoveDiretion.UP:
                    position = new Int2D( position.x, position.y - 1 );
                    break;
                default:
                    break;
            }

            // 일단 도착했으니까 제거
            if ( position.Equals( currentMovePath.Peek() ) )
                currentMovePath.Pop();

            // 몹이 있는지 확인한다
            // 있으면 일단 전투부터 요청
            Party target = dungeonMaster.GetMapObject( position.x, position.y ).party;
            if ( target != null && target.partyType == PartyType.MOB )
                dungeonMaster.StartBattle( target );

            // 아이템 있는지 확인한다
            Item item = (Item)dungeonMaster.GetMapObject( position.x, position.y ).gameObject;
            if ( item != null )
            {
                if ( item.code == ItemCode.Ring )
                    isRingDiscovered = true;
                else
                {
                    dungeonMaster.LootItem( item, currentZoneId );
                    UpdateDestination();
                }
            }

            // 현재 존 정보 업데이트 할 것
            int newZoneId = dungeonMaster.GetZoneId( position );

            if ( currentZoneId != newZoneId )
            {
                // 영역이 바뀌었다!
                currentZoneId = newZoneId;

                UpdateDestination();

                // 처음 가보는 곳이면 일단 스택에도 넣고, 가봤다고 기록도 하자
                if ( !exploredZone.Contains( currentZoneId ) )
                {
                    dungeonZoneHistory.Push( currentZoneId );
                    exploredZone.Add( currentZoneId );
                }
            }
        }

        private void UpdateDestination()
        {
            currentMovePath.Clear();

            // 남은 아이템이 있는지 확인하고
            // 남은 아이템 중에 제일 가까운 곳으로 방향을 정한다
            IEnumerable<Int2D> items =
                from item in dungeonMaster.GetItems( currentZoneId )
                let distance = Math.Abs( item.position.x - position.x ) + Math.Abs( item.position.y  - position.y )
                orderby distance
                select item.position;

            // 아이템이 있는 경우 줍고 간다
            if ( items.Count() > 0 )
                currentDestination = items.First();
            else
            {
                // 아이템이 없으면 다음 존으로 이동
                currentDestinationId = SelectNextZone();
                currentDestination = dungeonMaster.GetZonePosition( currentDestinationId );
            }

            MakePath( currentDestination );
        }

        // 다음 존 선택
        private int SelectNextZone()
        {
            // 현재 존에서 이동할 수 있는 존 확인
            foreach ( var each in dungeonMaster.GetLinkedZoneList( dungeonZoneHistory.Peek() ) )
            {
                // 그 중에서 탐색하지 않은 존 선택
                if ( !exploredZone.Contains( each ) )
                    return each;
            }

            // 만약 연결된 모든 존이 이미 방문한 존이라면 dungeonZoneHistory에서 현재 존을 pop하고 이전에 방문했던 존을 선택
            dungeonZoneHistory.Pop();

            return dungeonZoneHistory.Peek();
        }

        private float GetHeuristicScore( Int2D target, Int2D goal )
        {
            return (float)Math.Sqrt( Math.Pow( goal.x - target.x, 2 ) + Math.Pow( goal.y - target.y, 2 ) );
            // return Math.Abs( goal.x - target.x ) + Math.Abs( goal.y - target.y );
        }

        private void MakePath( Int2D destination )
        {
            // Console.WriteLine( "start to make the path" );
            // Console.WriteLine( "start : " + position.x + " / " + position.y );
            // Console.WriteLine( "dest : " + destination.x + " / " + destination.y );

            // 조심해!!!
            // priority_queue가 없어서 일단 list로 구현
            // List<ExploerNode> openSet = new List<ExploerNode>();
            MinHeap<ExploerNode> openSet = new MinHeap<ExploerNode>();

            for ( int i = 0; i < mapSize; ++i )
                for ( int j = 0; j < mapSize; ++j )
                    map[i, j].Reset();

            map[position.y, position.x].gScore = 0;
            map[position.y, position.x].fScore = map[position.y, position.x].gScore + GetHeuristicScore( position, destination );
            map[position.y, position.x].isOpened = true;
            // openSet.Add( map[position.y, position.x] );
            openSet.Push( map[position.y, position.x] );

            while( openSet.Count > 0 )
            {
                // ExploerNode current = openSet.Min();
                ExploerNode current = openSet.Peek();
                // Console.WriteLine( "current : " + current.position.x + " / " + current.position.y );

                if ( current.position.Equals( destination ) )
                {
                    currentMovePath.Push( currentDestination ); // 최종 목적지를 일단 넣고 그 사이를 채움

                    ReconstructPath( current );
                    currentMovePath.Pop();      // 현재 위치는 빼자
                    break;
                }

                // openSet.Remove( openSet.Min() );
                openSet.Pop();
                current.isClosed = true;

                // 조심해!
                // 배열을 하나 새로 만드는 것이 어색하다
                // 맨 끝 지점들에는 안 가니까 예외 처리는 생략
                List<ExploerNode> neigborNodes = new List<ExploerNode>{
                                                     map[current.position.y - 1, current.position.x],
                                                     map[current.position.y + 1, current.position.x],
                                                     map[current.position.y, current.position.x - 1],
                                                     map[current.position.y, current.position.x + 1]
                                                 };

                for ( int i = 0; i < neigborNodes.Count; ++i )
                {
                    ExploerNode neighbor = neigborNodes[i];

                    if ( neighbor.isClosed )
                        continue;

                    float gScoreTentative = current.gScore + Math.Abs( destination.x - current.position.x ) + Math.Abs( destination.y - current.position.y );

                    if ( !neighbor.isOpened || gScoreTentative < neighbor.gScore )
                    {
                        neighbor.cameFrom = current;
                        neighbor.gScore = gScoreTentative;
                        neighbor.fScore = neighbor.gScore + GetHeuristicScore( neighbor.position, destination );

                        if ( !neighbor.isOpened )
                        {
                            neighbor.isOpened = true;
                            // openSet.Add( neighbor );
                            openSet.Push( neighbor );
                        }
                        else
                            openSet.DecreaseKeyValue( neighbor );

                        // openSet.Sort( new NodeComp() );
                    }
                }
            }
            
            // Console.WriteLine( "end!!!!" );
        }

        private void ReconstructPath( ExploerNode currentNode )
        {
            if ( currentNode.cameFrom != null )
            {
                currentMovePath.Push( currentNode.cameFrom.position );
                ReconstructPath( currentNode.cameFrom );
            }
        }
    }
}
