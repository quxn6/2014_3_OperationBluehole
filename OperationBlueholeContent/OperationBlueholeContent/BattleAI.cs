using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
	enum ActionType : byte		// 이거 어디둘까...
	{
		None,
		PhyAttack,
		MagAttack,
		RecoverHp,
		RecoverMp,
		RecoverSp,
		Buff,
		Debuff,
		Escape,
		ActionTypeCount
	}

	enum AIConst : short
	{
		Default		= 0,

		AttackP		= 100,
		AtkExPoint	= 100,
		AtkMore		= 20,
		AtkAllMinP	= 20,
		AtkAllMinT	= 3,
		AtkAllP		= 100,

		RecoverP	= 100,
		RcvExPoint	= 100,
		RcvMinP		= 60,
		RcvMore		= 20,
		RcvAllMinT	= 3,
		RcvAllP		= 100,

		StyleP		= 10,
	}

	struct AIDecision{
		public ActionType type;
		public Character target;
		public short value;		// 우선도 판단용 수치

		public AIDecision(ActionType type, Character target, short value)
		{
			this.type = type;
			this.target = target;
			this.value = value;
		}
	}

	class BattleAI
	{
		private short randomLevel; // 판단의 랜덤성 수준... AI레벨 조절이 될듯?
// 		private short[] decisions;
		private List<AIDecision> decisions;
		private Character player;
		private Party ally, enemy;
		private Random random;

		public BattleAI(Random random, short randomLevel, Character player, Party ally, Party enemy)
		{
// 			decisions = new short[(int)ActionType.ActionTypeCount];
			decisions = new List<AIDecision>();
			ResetDecisions();

			this.random = random;
			this.randomLevel = randomLevel;
			this.player = player;
			this.ally = ally;
			this.enemy = enemy;
		}

		public bool Act(){
			CalcDecisions();	// 우선도 계산
			ApplyBattleStyle();	// 전투 성향에 따라 가중치
			Randomize();		// 랜덤 레벨에 의거한 랜덤 가중치

			// Decision들을 우선순위에 따라 하나씩 시도.
			foreach (var res in decisions.OrderBy(d => d.value))
			{
				{
					var resSkills = player.skills.Where(sid =>
						SkillManager.table[sid].type == res.type &&
						SkillManager.table[sid].hpNeed < player.hp &&
						SkillManager.table[sid].mpNeed <= player.mp &&
						SkillManager.table[sid].spNeed <= player.sp);
					if (res.target == null)
						resSkills = resSkills.Where(sid => SkillManager.table[sid].targetType == TargetType.All);

					int skillCnt = resSkills.Count();
					if (skillCnt > 0)
					{
						var resSkill = resSkills.ElementAt(random.Next(0, skillCnt - 1));
						SkillManager.table[resSkill].Act(random, player, res.target);

						ResetDecisions();
						return true;
					}
				}
				{
					var resItems = player.items.Where(iid =>
						((Consumable)ItemManager.table[iid]).type == res.type &&
						((Consumable)ItemManager.table[iid]).spNeed <= player.sp);
					if (res.target == null)
						resItems = resItems.Where(iid => ((Consumable)ItemManager.table[iid]).targetType == TargetType.All);

					int itemCnt = resItems.Count();
					if (itemCnt > 0)
					{
						var resItem = resItems.ElementAt(random.Next(0, itemCnt - 1));
						((Consumable)ItemManager.table[resItem]).UseItem(random, player, res.target);

						ResetDecisions();
						return true;
					}
				}
			}

			// 아무 것도 할 수 없었다면 false 리턴하여 턴 종료
			ResetDecisions();
			return false;
		}

		void CalcDecisions()
		{
			// 아군 회복 우선도 체크
			foreach (var target in ally.characters)
			{
				short value = (short)((short)AIConst.RecoverP - target.hp * (short)AIConst.RecoverP / target.actualParams[(int)ParamType.maxHp]);
				
				if (value > (short)AIConst.RcvMinP)
					decisions.Add(new AIDecision(
						ActionType.RecoverHp,
						target,
						value
						));
				
				if (enemy.characters.Average(c => 
					c.actualParams[(int)ParamType.phyAtk] + c.actualParams[(int)ParamType.magAtk]
					) >= target.hp)
				{
					value += (short)AIConst.RcvExPoint;
				}
			}

			// 범위 회복 우선도 체크
			if (decisions.Where(d => d.type == ActionType.RecoverHp)
				.Count() >= (int)AIConst.RcvAllMinT)
			{
				decisions.Add(new AIDecision(
					ActionType.RecoverHp,
					null,
					(short)AIConst.RcvAllP
					));
			}

			// 적 공격 우선도 체크
			foreach (var target in enemy.characters)
			{
				short value = (short)((short)AIConst.AttackP - target.hp * (short)AIConst.AttackP / target.actualParams[(int)ParamType.maxHp]);

				AIDecision phy = new AIDecision(
					ActionType.PhyAttack,
					target,
					value);
				AIDecision mag = new AIDecision(
					ActionType.MagAttack,
					target,
					value);
				
				uint phyDmg = player.actualParams[(int)ParamType.phyAtk] - target.actualParams[(int)ParamType.phyDef];
				uint magDmg = player.actualParams[(int)ParamType.magAtk] - target.actualParams[(int)ParamType.magDef];

				if (phyDmg > target.hp)
					phy.value += (short)AIConst.AtkExPoint;
				else if (magDmg > target.hp)
					mag.value += (short)AIConst.AtkExPoint;
				else
				{
					if (phyDmg >= magDmg)
						phy.value = (short)AIConst.AtkMore;
					else
						mag.value = (short)AIConst.AtkMore;
				}

				decisions.Add(phy);
				decisions.Add(mag);
			}

			// 범위 공격 우선도 체크
			if (decisions.Where(d => d.type == ActionType.PhyAttack && d.value > (short)AIConst.AtkAllMinP)
				.Count() >= (int)AIConst.AtkAllMinT)
			{
				decisions.Add(new AIDecision(
					ActionType.PhyAttack,
					null,
					(short)AIConst.AtkAllP
					));
			}
			if (decisions.Where(d => d.type == ActionType.MagAttack && d.value > (short)AIConst.AtkAllMinP)
				.Count() >= (int)AIConst.AtkAllMinT)
			{
				decisions.Add(new AIDecision(
					ActionType.MagAttack,
					null,
					(short)AIConst.AtkAllP
					));
			}

			var buffs = player.skills.Where(sid => SkillManager.table[sid].type == ActionType.Buff);
			foreach (var buff in buffs)
			{
				//TODO: 각 버프들에 대한 아군 체크
			}

			var debuffs = player.skills.Where(sid => SkillManager.table[sid].type == ActionType.Debuff);
			foreach (var buff in buffs)
			{
				//TODO: 각 디버프들에 대한 적군 체크
			}
		}

		void ApplyBattleStyle()
		{
			IEnumerable<AIDecision> res = null;
			switch (player.battleStyle)
			{
				case BattleStyle.AGGRESSIVE:
					res = decisions.Where(d => d.type == ActionType.PhyAttack || d.type == ActionType.MagAttack);
					break;
				case BattleStyle.DEFENSIVE:
					res = decisions.Where(d => d.type == ActionType.RecoverHp);
					break;
				case BattleStyle.SUPPORTIVE:
					res = decisions.Where(d => d.type == ActionType.Buff || d.type == ActionType.Debuff);
					break;
			}
			
			AIDecision tmp;
			foreach (var d in res)
			{
				tmp = d;
				tmp.value += (short)AIConst.StyleP;
			}
		}

		void Randomize()
		{
// 			decisions = decisions.ConvertAll(d =>{ 
// 				d.value += (short)random.Next(0, randomLevel); 
// 				return d;
// 			});
			AIDecision tmp;
			foreach (var d in decisions)
			{
				tmp = d;
				tmp.value += (short)random.Next(0, randomLevel);
			}
		}

		void ResetDecisions()
		{
// 			for (int i = 0; i < (int)ActionType.ActionTypeCount; ++i)
// 			{
// 				decisions[i] = 0;
// 			}
			decisions.Clear();
		}
	}
}
