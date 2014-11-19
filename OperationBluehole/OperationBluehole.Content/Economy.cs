using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationBluehole.Content
{
    public class ItemToken : Item
    {
        static readonly EquipType[] equipTypePool = { 
                                                        EquipType.Head, 
                                                        EquipType.Body, 
                                                        EquipType.LHand, 
                                                        EquipType.RHand, 
                                                        EquipType.Leg, 
                                                        EquipType.Feet, 
                                                    };
        
        public readonly int level;
        public readonly EquipType equipType;

        public ItemToken( int level, RandomGenerator random )
        {
            code = ItemCode.Token;
            this.level = level;
            this.equipType = equipTypePool[random.Next( equipTypePool.Length )];
        }
    }
}
