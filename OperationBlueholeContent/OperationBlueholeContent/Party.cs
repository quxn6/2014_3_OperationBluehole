﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
	// 파티원들의 목록과 차후 아이템 분배라던가의 속성도 넣어야 할지도?
    enum PartyType
    {
        PLAYER,
        MOB
    }

    class Party
    {
        public List<Character> characters = new List<Character>();
        public PartyType partyType;

        public Party( PartyType type )
        {
            this.partyType = type;
        }

        public void AddCharacter(Character newMember)
        {
            characters.Add(newMember);
        }
    }
}
