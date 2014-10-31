using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
	enum SkillType : byte
	{
		None,
		Attack,
		Heal,
		Buff,
		Debuff,
	}
	enum SkillTargetType : byte
	{
		None,		// 타겟 지정 없음
		Single,		// 1명 지정 스킬
		Multiple,	// 여러명 지정 스킬
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
	
	class Skill
	{
		public SkillType type { get; private set; }
		public SkillTargetType targetType { get; private set; }
		public string name { get; private set; }
		public uint hpNeed { get; private set; }
		public uint mpNeed { get; private set; }
		public uint spNeed { get; private set; }
		private Func<Character, Character, bool> action;

		public Skill(SkillType type, SkillTargetType targetType, uint hpNeed, uint mpNeed, uint spNeed, Func<Character, Character, bool> action)
		{
			this.type = type;
			this.targetType = targetType;
			this.hpNeed = hpNeed;
			this.mpNeed = mpNeed;
			this.spNeed = spNeed;
			this.action = action;
		}

		public bool Act(Character src)
		{
			if (targetType != SkillTargetType.None)
				return false;

			if (src.ReduceForAction(hpNeed, mpNeed, spNeed))
				return action(src, null);

			return false;
		}

		public bool Act(Character src, Character target)
		{
			if (targetType != SkillTargetType.Single)
				return false;

			if (src.ReduceForAction(hpNeed, mpNeed, spNeed))
				return action(src, target);

			return false;
		}

		public bool Act(Character src, Character[] targets)
		{
			if (targetType != SkillTargetType.Multiple)
				return false;

			if (src.ReduceForAction(hpNeed, mpNeed, spNeed))
			{
				foreach (Character target in targets)
					action(src, target);
				return true;
			}

			return false;
		}
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
					SkillType.Attack,
					SkillTargetType.Single,
					0,
					0,
					50,
					delegate(Character src, Character target)
					{
						uint accuracy = (uint)(src.baseStats[(int)StatType.Dex]);
						if (target.HitCheck(HitType.Melee, accuracy))
						{
							uint damage = src.baseStats[(int)StatType.Str];
							target.Hit(HitType.Melee, damage);
						}
						return true;
					}
					)
				);

			SkillManager.table.Add(SkillId.Punch,
				new Skill(
					SkillType.Attack,
					SkillTargetType.Single,
					0,
					0,
					50,
					delegate(Character src, Character target)
					{
						uint accuracy = (uint)(src.baseStats[(int)StatType.Dex]*0.9);
						if (target.HitCheck(HitType.Melee, accuracy))
						{
							uint damage = src.baseStats[(int)StatType.Str];
							target.Hit(HitType.Melee, damage);
						}
						return true;
					}
					)
				);

			SkillManager.table.Add(SkillId.MagicArrow,
				new Skill(
					SkillType.Attack,
					SkillTargetType.Single,
					0,
					10,
					50,
					delegate(Character src, Character target)
					{
						uint accuracy = (uint)(src.baseStats[(int)StatType.Wis] * 0.4 + src.baseStats[(int)StatType.Int] * 0.6);
						if (target.HitCheck(HitType.Magical, accuracy))
						{
							uint damage = src.baseStats[(int)StatType.Int];
							target.Hit(HitType.Magical, src.baseStats[(int)StatType.Int]);
						}
						return true;
					}
					)
				);

			SkillManager.table.Add(SkillId.Heal,
				new Skill(
					SkillType.Heal,
					SkillTargetType.Single,
					0,
					10,
					50,
					delegate(Character src, Character target)
					{
						uint damage = src.baseStats[(int)StatType.Int];
						target.Recover(GaugeType.Hp, damage);
						return true;
					}
					)
				);

		}
	}
}
