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

    // 시야 개념은 던전 내부 zone으로 구분
    // zone 정보는 DM를 통해서 얻음
    class Explorer
    {
        public Int2D position { get; set; }
        public Int2D currentDestination { get; private set; }
        public int currentDestinationId { get; private set; }

        private Queue<Int2D> currentMovePath;

        Stack<int>      dungeonZoneHistory;
        HashSet<int>    exploredZone;

        DungeonMaster dungeonMaster;

        public Explorer( DungeonMaster master )
        {
            this.dungeonMaster = master;
        }

        public void Init( Int2D position )
        {
            this.position = position;

            // 존 방문 기록도 업데이트하고, 첫 movePath 계산도 해둔다

        }

        // watch out!
        // FOR DEBUG!!!!
        public void Teleport( Int2D destination )
        {
            position = destination;
        }

        public int GetCurrentZoneId() { return dungeonZoneHistory.Peek(); }



        public MoveDiretion Move()
        {
            if ( position.Equals( currentMovePath.Peek() ) )
                currentMovePath.Dequeue();

            // 도착했거나 작성된 경로가 없는 경우 새로 생성
            if ( position.Equals( currentDestination ) || currentMovePath.Count == 0 )
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

        private void UpdateDestination()
        {
            currentMovePath.Clear();

            // 아이템이 있는 경우 거쳐서 갈 지 결정해야 함

            currentDestinationId = SelectNextZone();
            currentDestination = dungeonMaster.GetZonePosition( currentDestinationId );
        }

        private void MakePath( Int2D destination )
        {
            currentDestination = destination;

            // 주어진 좌표로의 최단 경로를 생성
            // 이건 A* algorithm 사용하면 될 듯
        }
    }
}
