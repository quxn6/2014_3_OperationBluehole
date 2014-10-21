using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public bool Init( Party users, int size )
        {
            this.users = users;

            // 일단은 빈 리스트들이지만 던전이 생성되고 나면 내부에서 배치된 것들이 안에 등록된다.
            List<Party> mobs = new List<Party>();
            List<Item> items = new List<Item>();

            dungeon = new Dungeon( size, mobs, items, users );

            return true;
        }

        public void Start()
        {
            while ( true )
            {
                break;
            }
        }
    }
}
