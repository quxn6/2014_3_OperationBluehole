using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
    enum GameObjectId
    {
        VOID = 0,
        WALL,
        ITEM,
        MOB,
        PLAYER
    };

    class GameObject
    {
        public GameObjectId id { get; set; }
    }

    class Player : GameObject
    {
        public Player()
        {
            id = GameObjectId.PLAYER;    
        }
    }

    class Mob : GameObject
    {

    }
}
