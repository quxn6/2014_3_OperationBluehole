using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
	class Battle
	{
		public Party[] party { get; private set; }
		public int curTurn { get; private set; }
		public int battleResult { get; private set; }
		private Random random;

		public Battle(Random random, Party party1, Party party2)
		{
			this.random = random;

			party = new Party[2];
			party[0] = party1;
			party[1] = party2;

			curTurn = 1;
			battleResult = 0;
		}

		public void StartBattle()
		{
			while (battleResult == 0)
			{
				// 전투상 모든 캐릭터들에 턴행동 후 전멸 체크
				foreach (Character i in party[0].characters)
				{
					if (i.hp > 0)
						TurnAction(i, true);
					if (EndCheck())
						return;
				}
                foreach ( Character i in party[1].characters )
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
                turnCharacter.BattleTurnAction( random, party[0], party[1] );
			else
                turnCharacter.BattleTurnAction( random, party[1], party[0] );
		}

		// 전투 종료 조건 체크( 상대 파티의 전멸 )
		// mBattleResult에 이긴 파티의 번호를 저장.
		bool EndCheck()
		{
            if ( party[0].characters.Sum( chr => chr.hp ) <= 0 )
			{
				battleResult = 2;
				return true;
			}
            if ( party[1].characters.Sum( chr => chr.hp ) <= 0 )
			{
				battleResult = 1;
				return true;
			}
			return false;
		}
	}
}
