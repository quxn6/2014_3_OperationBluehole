using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
    enum PartyIndex
    {
        NONE = -1,
        USERS = 0,
        MOBS = 1,
    }

	class Battle
	{
		public Party[] party { get; private set; }
		public int curTurn { get; private set; }
        public PartyIndex battleResult { get; private set; }
        private RandomGenerator random;

        public Battle( RandomGenerator random, Party party1, Party party2 )
		{
			this.random = random;

			party = new Party[2];
			party[0] = party1;
			party[1] = party2;

			curTurn = 1;
            battleResult = PartyIndex.NONE;
		}

		public void StartBattle()
		{
            while ( battleResult == PartyIndex.NONE )
			{
				// 전투상 모든 캐릭터들에 턴행동 후 전멸 체크
                foreach ( Character i in party[(int)PartyIndex.USERS].characters )
				{
					if (i.hp > 0)
						TurnAction(i, true);
					if (EndCheck())
						return;
				}
                foreach ( Character i in party[(int)PartyIndex.MOBS].characters )
				{
					if (i.hp > 0)
						TurnAction(i, false);
					if (EndCheck())
						return;
				}

				++curTurn;
			}
		}

		// 매턴마다 SP를 증가시켜주고 SP 100이 되면 해당 캐릭터 행동 실행
		void TurnAction(Character turnCharacter, bool isUserParty)
		{
			if (turnCharacter.sp < 100)
			{
				turnCharacter.Rest();
				return;
			}

			if (isUserParty)
                turnCharacter.BattleTurnAction( random, party[(int)PartyIndex.USERS], party[(int)PartyIndex.MOBS] );
			else
                turnCharacter.BattleTurnAction( random, party[(int)PartyIndex.MOBS], party[(int)PartyIndex.USERS] );
		}

		// 전투 종료 조건 체크( 상대 파티의 전멸 )
		// mBattleResult에 이긴 파티의 번호를 저장.
		bool EndCheck()
		{
            if ( party[(int)PartyIndex.USERS].characters.Sum( chr => chr.hp ) <= 0 )
			{
                battleResult = PartyIndex.MOBS;
				return true;
			}
            if ( party[(int)PartyIndex.MOBS].characters.Sum( chr => chr.hp ) <= 0 )
			{
				battleResult = PartyIndex.USERS;
				return true;
			}
			return false;
		}
	}
}
