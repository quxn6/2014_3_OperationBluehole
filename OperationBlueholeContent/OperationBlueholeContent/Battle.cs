using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
    class Battle
    {
		public Party[] mParty { get; private set; }
		public int mCurTurn { get; private set; }
		public int mBattleResult { get; private set; }

        public Battle( Party party1, Party party2 )
        {
			mParty = new Party[2];
            mParty[0] = party1;
            mParty[1] = party2;

			mCurTurn = 1;
			mBattleResult = 0;
        }

        public void StartBattle()
        {
			while (mBattleResult == 0)
            {
				// 전투상 모든 캐릭터들에 턴행동 후 전멸 체크
                foreach (Character i in mParty[0].mCharacters)
                {
					if (i.mHpCur > 0)
						TurnAction(i, 0);
					if (EndCheck())
						return;
                }
                foreach (Character i in mParty[1].mCharacters)
                {
					if (i.mHpCur > 0)
						TurnAction(i, 1);
					if (EndCheck())
						return;
                }

				++mCurTurn;
            }
        }

		// 매턴마다 SP를 증가시켜주고 SP 100이 되면 해당 캐릭터 행동 실행
        void TurnAction( Character turnCharacter, int partyNum )
        {
			if (turnCharacter.mSpCur < 100)
			{
				turnCharacter.Rest();
				return;
			}

            if (partyNum == 0)
                turnCharacter.BattleTurnAction(mParty[0], mParty[1]);
            else
                turnCharacter.BattleTurnAction(mParty[1], mParty[0]);
        }

		// 전투 종료 조건 체크( 상대 파티의 전멸 )
		// mBattleResult에 이긴 파티의 번호를 저장.
		bool EndCheck()
		{
			if (mParty[0].mCharacters.Sum(chr => chr.mHpCur) <= 0)
			{
				mBattleResult = 2;
				return true;
			}
			if (mParty[1].mCharacters.Sum(chr => chr.mHpCur) <= 0)
			{
				mBattleResult = 1;
				return true;
			}
			return false;
		}
    }
}
