using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
	// 전투 성향, 차후 AI행동시 사용...되려나
	enum BattleStyle
	{
		AGGRESSIVE,
		DEFENSIVE,
		SUPPORTIVE
	}
	// 플레이 성향
	enum PlayStyle
	{
		SELFISH,
		NEUTRAL,
		SELFLESS
	}

	class Character : GameObject
    {
		public short mLev { get; private set; }
		public short mStr { get; private set; }
		public short mDex { get; private set; }
		public short mInt { get; private set; }
		public short mCon { get; private set; }
		public short mAgi { get; private set; }
		public short mWis { get; private set; }
		public short mMov { get; private set; }
		
		public int mHp { get; private set; }
		public int mMp { get; private set; }
		public int mHpCur { get; private set; }
		public int mMpCur { get; private set; }
        public int mSpCur { get; private set; }


        public Character()
        {
            mLev = 1;
            mHpCur = mHp = 100;
            mMpCur = mMp = 100;
			mSpCur = 0;

			mStr = mDex = mInt = mCon = mAgi = mWis = 10;
			mMov = 5;
        }

		// 현재 휴식하면 SP가 1씩 차도록...
        public void Rest()
        {
            ++mSpCur;
            if (mSpCur > 100)
                mSpCur = 100;
        }

		// 차후 적과 아군의 파티 정보를 보고 행동을 결정하는 AI 추가 필요
		// 현재는 그냥 가장 체력 낮은 적부터 다굴...
        public void BattleTurnAction( Party Ally, Party Enemy )
        {
            Character weakEnemy = Enemy.characters.Where( c => c.mHpCur > 0).OrderBy(c => c.mHpCur).First();
			Attack(weakEnemy);
        }

		// 공격시 스탯/장비에 의한 공격력 계산, 명중/회피, 
		// 스킬 등의 공식 적용등이 필요하지만 일단은 그냥 단순 str타격
		public void Attack( Character target )
		{
			mSpCur -= 50;
			target.Damage(mStr);
		}
        
		public void Damage( int damage )
        {
            mHpCur -= damage;
            if (mHpCur <= 0)
                mHpCur = 0;
        }
    }

	class Player : Character
	{
		public int mChrId { get; private set; }
		public int mExp { get; private set; }

		public Player()
		{
			mChrId = 0;
			mExp = 0;
		}
	}

	// 몹에는 드랍 아이템, 경험치 같은게 들어가야하려나
	class Mob : Character
	{

	}
}
