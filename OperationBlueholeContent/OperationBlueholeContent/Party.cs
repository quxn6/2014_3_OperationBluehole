using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
	// 파티원들의 목록과 차후 아이템 분배라던가의 속성도 넣어야 할지도?
    // 서버에서 쓰려니 네임스페이스가 달라서 일단 public을 붙임.
    public enum PartyType
    {
        PLAYER,
        MOB
    }

    // 서버에서 쓰려니 네임스페이스가 달라서 일단 public을 붙임.
    public class Party
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
