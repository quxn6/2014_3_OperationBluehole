﻿using System;
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

    // 기본적으로 그냥 바닥, 벽, 아무것도 아닌 것이 있는데, ID 보고 판단
    // TILE인 경우에는 gameObject가 null인지 보고, 아니면 해당 오브젝트를 렌더?
    class MapObject
    {
        public MapObjectType objectType;
        public GameObject gameObject;
        public Party party { get; set; }

        public MapObject( MapObjectType id, GameObject obj )
        {
            this.objectType = id;
            this.gameObject = obj;
        }
    }
}