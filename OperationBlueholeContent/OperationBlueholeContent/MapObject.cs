using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
    enum MapObjectType
    {
        VOID = 0,
        TILE,
        WALL,
    }

    class MapObject
    {
        public MapObjectType objectType { get; set; }
        public GameObject gameObject { get; set; }
        public Party party { get; set; }
        public int zoneId { get; set; }

        public MapObject() 
        {
            objectType = MapObjectType.VOID;
            zoneId = -1;
        }

        public void Reset()
        {
            objectType = MapObjectType.VOID;
            gameObject = null;
            party = null;
            zoneId = -1;
        }
    }
}
