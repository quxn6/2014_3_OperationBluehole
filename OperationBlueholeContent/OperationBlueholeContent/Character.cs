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
		public short statLev { get; private set; }
		public short statStr { get; private set; }
		public short statDex { get; private set; }
		public short statInt { get; private set; }
		public short statCon { get; private set; }
		public short statAgi { get; private set; }
		public short statWis { get; private set; }
		public short statMov { get; private set; }

		public int statHp { get; private set; }
		public int statMp { get; private set; }
		public int hp { get; private set; }
		public int mp { get; private set; }
		public int sp { get; private set; }


		public Character()
		{
			statLev = 1;
			hp = statHp = 100;
			mp = statMp = 100;
			sp = 0;

			statStr = statDex = statInt = statCon = statAgi = statWis = 10;
			statMov = 5;
		}

		// 현재 휴식하면 SP가 1씩 차도록...
		public void Rest()
		{
			++sp;
			if (sp > 100)
				sp = 100;
		}

		// 차후 적과 아군의 파티 정보를 보고 행동을 결정하는 AI 추가 필요
		// 현재는 그냥 가장 체력 낮은 적부터 다굴...
		public void BattleTurnAction(Party Ally, Party Enemy)
		{
			Character weakEnemy = Enemy.mCharacters.Where(c => c.hp > 0).OrderBy(c => c.hp).First();
			Attack(weakEnemy);
		}

		// 공격시 스탯/장비에 의한 공격력 계산, 명중/회피, 
		// 스킬 등의 공식 적용등이 필요하지만 일단은 그냥 단순 str타격
		public void Attack(Character target)
		{
			sp -= 50;
			target.Damage(statStr);
		}

		public void Damage(int damage)
		{
			hp -= damage;
			if (hp <= 0)
				hp = 0;
		}
	}

	class Player : Character
	{
		public int id { get; private set; }
		public int exp { get; private set; }

		public Player()
		{
			id = 0;
			exp = 0;
		}
	}

	// 몹에는 드랍 아이템, 경험치 같은게 들어가야하려나
	class Mob : Character
	{

	}
}
