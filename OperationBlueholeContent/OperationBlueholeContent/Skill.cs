using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
	enum TargetType : byte
	{
		None,		// 타겟 지정 없음
		Single,		// 1명 지정 스킬
// 		Multiple,	// 여러명 지정 스킬
		All,		// 전체 스킬
	}
	enum SkillId : ushort
	{
		None = 0,
		Slash,
		Punch,
		Heal,
		MagicArrow,
	}
	
	internal class Skill
	{
		public ActionType type { get; private set; }
		public TargetType targetType { get; private set; }
		public string name { get; private set; }
		public uint hpNeed { get; private set; }
		public uint mpNeed { get; private set; }
		public uint spNeed { get; private set; }
        private Func<RandomGenerator, Character, Character, bool> action;

        public Skill( ActionType type, TargetType targetType, uint hpNeed, uint mpNeed, uint spNeed, Func<RandomGenerator, Character, Character, bool> action )
		{
			this.type = type;
			this.targetType = targetType;
			this.hpNeed = hpNeed;
			this.mpNeed = mpNeed;
			this.spNeed = spNeed;
			this.action = action;
		}

        public bool Act( RandomGenerator random, Character src )
		{
			if (targetType != TargetType.None && targetType != TargetType.All)
				return false;

			if (src.ReduceForAction(hpNeed, mpNeed, spNeed))
				return action(random, src, null);

			return false;
		}

        public bool Act( RandomGenerator random, Character src, Character target )
		{
			if (target == null)
				return Act(random, src);

			if (targetType != TargetType.Single)
				return false;

			if (src.ReduceForAction(hpNeed, mpNeed, spNeed))
				return action(random, src, target);

			return false;
		}

// 		public bool Act(Random random, Character src, Character[] targets)
// 		{
// 			if (targetType != SkillTargetType.Multiple)
// 				return false;
// 
// 			if (src.ReduceForAction(hpNeed, mpNeed, spNeed))
// 			{
// 				foreach (Character target in targets)
// 					action(random, src, target);
// 				return true;
// 			}
// 
// 			return false;
// 		}
	}

	// 이거 뭔가 찝찝한데...
	static class SkillManager
	{
		//public static List<Skill> skillList { get; private set; }

		public static Dictionary<SkillId, Skill> table { get; private set; }
		public static void Init()
		{
// 			SkillManager.skillList = new List<Skill>();
// 
// 			SkillManager.skillList[(ushort)SkillName.Slash] = new Skill(
// 				"Slash",
// 				SkillType.Attack,
// 				SkillTargetType.Single,
// 				0,
// 				0,
// 				50,
// 				delegate(Character src, Character target)
// 				{
// 					target.Damage(src.statStr);
// 					return true;
// 				}
// 				);
// 
// 			SkillManager.skillList[(ushort)SkillName.Heal] = new Skill(
// 				"Heal",
// 				SkillType.Heal,
// 				SkillTargetType.Single,
// 				0,
// 				10,
// 				50,
// 				delegate(Character src, Character target)
// 				{
// 					target.Recover(src.statInt);
// 					return true;
// 				}
// 				);

			SkillManager.table = new Dictionary<SkillId, Skill>();

			SkillManager.table.Add(SkillId.Slash,
				new Skill(
					ActionType.PhyAttack,
					TargetType.Single,
					0,
					0,
					50,
                    delegate( RandomGenerator random, Character src, Character target )
					{
						uint accuracy = (uint)(src.baseStats[(int)StatType.Dex]);
						if (target.HitCheck(HitType.Melee, accuracy))
						{
							int damage = src.baseStats[(int)StatType.Str];
							int maxDamage = (int)(damage * 1.2f);
							damage = (int)(damage * 0.8f);
							damage = random.Next(damage, maxDamage);
							target.Hit(HitType.Melee, (uint)damage);
						}
						return true;
					}
					)
				);

			SkillManager.table.Add(SkillId.Punch,
				new Skill(
					ActionType.PhyAttack,
					TargetType.Single,
					0,
					0,
					50,
                    delegate( RandomGenerator random, Character src, Character target )
					{
						uint accuracy = (uint)(src.baseStats[(int)StatType.Dex]*0.9);
						if (target.HitCheck(HitType.Melee, accuracy))
						{
							int damage = src.baseStats[(int)StatType.Str];
							int maxDamage = (int)(damage * 1.2f);
							damage = (int)(damage * 0.8f);
							damage = random.Next(damage, maxDamage);
							target.Hit(HitType.Melee, (uint)damage);
						}
						return true;
					}
					)
				);

			SkillManager.table.Add(SkillId.MagicArrow,
				new Skill(
					ActionType.MagAttack,
					TargetType.Single,
					0,
					10,
					50,
                    delegate( RandomGenerator random, Character src, Character target )
					{
						uint accuracy = (uint)(src.baseStats[(int)StatType.Wis] * 0.4 + src.baseStats[(int)StatType.Int] * 0.6);
						if (target.HitCheck(HitType.Magical, accuracy))
						{
							int damage = src.baseStats[(int)StatType.Int];
							int maxDamage = (int)(damage * 1.2f);
							damage = (int)(damage * 0.8f);
							damage = random.Next(damage, maxDamage);
							target.Hit(HitType.Magical, (uint)damage);
						}
						return true;
					}
					)
				);

			SkillManager.table.Add(SkillId.Heal,
				new Skill(
					ActionType.RecoverHp,
					TargetType.Single,
					0,
					10,
					50,
                    delegate( RandomGenerator random, Character src, Character target )
					{
						int damage = src.baseStats[(int)StatType.Int];
						int maxDamage = (int)(damage * 1.2f);
						damage = (int)(damage * 0.8f);
						damage = random.Next(damage, maxDamage);
						target.Recover(GaugeType.Hp, (uint)damage);
						return true;
					}
					)
				);

		}
	}
}
