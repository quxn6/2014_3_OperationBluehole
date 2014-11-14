using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
    // 서버에서 쓰려니 네임스페이스가 달라서 일단 public을 붙임.
    public enum BuffCode : uint
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
