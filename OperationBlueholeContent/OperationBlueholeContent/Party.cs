using System;
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
        public Int2D position { get; set; }
        public int partyLevel { get; private set; }

        public Party( PartyType type, int partyLevel )
        {
            this.partyType = type;
            this.partyLevel = partyLevel;
        }

        public void AddCharacter(Character newMember)
        {
            characters.Add(newMember);
        }
    }
}
