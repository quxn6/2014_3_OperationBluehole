using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
    enum BuffCode : uint
    {
        None = 0x0,
    }

    class Buff
    {
        public uint duration { get; protected set; }
        public List<Tuple<StatType, ushort>> plusStat { get; private set; }
        public List<Tuple<StatType, uint>> plusParam { get; private set; }
    }

    static class BuffManager
    {
        public static Dictionary<BuffCode, Buff> table { get; private set; }

        public static void Init()
        {
            BuffManager.table = new Dictionary<BuffCode, Buff>();

            // sample buff
            /*
            BuffManager.table.Add( ItemCode.HpPotionS,
                new Consumable( ItemCode.HpPotionS, ItemType.Consume,
                    50,
                    delegate( Random random, Character src, Character target )
                    {
                        target.Recover( GaugeType.Hp, 50 );
                        return true;
                    }
                ) );

            BuffManager.table.Add( ItemCode.MpPotionS,
                new Consumable( ItemCode.MpPotionS, ItemType.Consume,
                    50,
                    delegate( Random random, Character src, Character target )
                    {
                        target.Recover( GaugeType.Mp, 20 );
                        return true;
                    }
                ) );
             */ 
        }
    }
}
