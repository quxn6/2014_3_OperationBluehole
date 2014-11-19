using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Content
{
    public class GameResult
    {
        public struct MapInfo
        {
            public int type;        // map object type
            public int itemIndex;   // nothing = -1
            public int mobIndex;    // nothing = -1
        }

        public struct ItemInfo
        {
            public uint code;
            public int level;
            public ushort equiptype;
        }

        public struct MobInfo
        {
            public int Lev;
        }

        public struct Position
        {
            public int x, y;
            public int battleIdx;   // nothing = -1, 일단 따로 기록 안 하고 몹 만나면 차례대로 접근하게 할까
        }

        public struct BattleInfo
        {
        }

        // player data
            // List<PlayerData>
        private int mapSize = 0;

        public int MapSize
        {
            get
            {
                return mapSize;
            }

            set
            {
                mapSize = value;
                Map = new MapInfo[value * value];
            }
        }
        public MapInfo[] Map;

        public List<ItemInfo> ItemList = new List<ItemInfo>();    // item code
        public List<List<MobInfo>> MobList = new List<List<MobInfo>>();

        public List<Position> Pathfinding = new List<Position>();

        // battle
        public List<BattleInfo> BattleList = new List<BattleInfo>();

        // 루팅한 아이템, 경험치, 골드 기록들도 추가할 것
    }
}
