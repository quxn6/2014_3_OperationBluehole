using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
	// 파티원들의 목록과 차후 아이템 분배라던가의 속성도 넣어야 할지도?
    class Party
    {
        public List<Character> mCharacters = new List<Character>();

        public void AddCharacter(Character newMember)
        {
            mCharacters.Add(newMember);
        }
    }
}
