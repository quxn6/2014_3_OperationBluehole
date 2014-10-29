using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
    enum ItemCode
    {
        NOTHING = -1,
        RING,
        DEBUG,
    }

    class Item : GameObject
    {
        public ItemCode code = ItemCode.NOTHING;
    }

    // 이걸 획득하면 게임 종료
    class RingOfErrethAkbe : Item
    {
        public RingOfErrethAkbe()
        {
            code = ItemCode.RING;
        }
    }
}
