using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationBluehole.Content
{
    using Newtonsoft.Json;

    public class ItemToken : Item
    {
        static public readonly EquipType[] equipTypePool = { 
                                                        EquipType.Head, 
                                                        EquipType.Body, 
                                                        EquipType.LHand, 
                                                        EquipType.RHand, 
                                                        EquipType.Leg, 
                                                        EquipType.Feet, 
                                                    };

        public int level { get; set; }
        public EquipType equipType { get; set; }

        public void GenerateRandomToken( int level, EquipType type )
        {
            code = ItemCode.Token;
            this.level = level;
            this.equipType = type;
        }
    }
}
