using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Content
{
    public class GameRecord
    {
        // 참조할 정보 : path, battle log, looted items, looted gold, looted exp
        public struct BattleInfo
        {
        }

        // 과정
        public List<Int2D> pathfinding = new List<Int2D>();
        public List<BattleInfo> battleLog = new List<BattleInfo>();

        // 전리품
        public List<Item> lootedItems = new List<Item>();
        public int lootedGold = 0;
        public int lootedExp = 0;
    }
}
