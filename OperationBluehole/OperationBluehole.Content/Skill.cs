using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Content
{
	enum TargetType : byte
	{
		None,		// 타겟 지정 없음
		Single,		// 1명 지정 스킬
// 		Multiple,	// 여러명 지정 스킬
		All,		// 전체 스킬
	}
    // 서버에서 쓰려니 네임스페이스가 달라서 일단 public을 붙임.
    public enum SkillId : ushort
	{
		None = 0,
		Slash,
		Punch,
		Heal,
		MagicArrow,
		Bite,
	}
	
	internal class Skill
	{
		public ActionType type { get; private set; }
		public TargetType targetType { get; private set; }
		public WeaponType weaponType { get; private set; }
		public string name { get; private set; }
		public uint hpNeed { get; private set; }
		public uint mpNeed { get; private set; }
		public uint spNeed { get; private set; }
        private Func<RandomGenerator, Character, Character, bool> action;

        public Skill( ActionType type, 
			TargetType targetType,
			WeaponType weaponType,
			uint hpNeed, uint mpNeed, uint spNeed, 
			Func<RandomGenerator, Character, Character, bool> action )
		{
			this.type = type;
			this.targetType = targetType;
			this.weaponType = weaponType;
			this.hpNeed = hpNeed;
			this.mpNeed = mpNeed;
			this.spNeed = spNeed;
			this.action = action;
		}

        public bool Act( RandomGenerator random, Character src )
		{
			if (targetType != TargetType.None)
				return false;
			if (WeaponCheck(src))
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
			if (!WeaponCheck(src))
				return false;

			if (src.ReduceForAction(hpNeed, mpNeed, spNeed))
				return action(random, src, target);

			return false;
		}

		public bool Act(RandomGenerator random, Character src, List<Character> targets)
		{
			if (targets.Count == 1)
				return Act(random, src, targets.First());

			if (targetType != TargetType.All)
				return false;
			if (WeaponCheck(src))
				return false;

			if (src.ReduceForAction(hpNeed, mpNeed, spNeed))
			{
				foreach (Character target in targets)
					action(random, src, target);
				return true;
			}

			return false;
		}
		bool WeaponCheck(Character src)
		{
			if ((src.weaponStatus & weaponType) > 0)
				return true;
			else
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
					ActionType.PhyAttack,
					TargetType.Single,
					WeaponType.Sword,
					0,
					0,
					45,
                    delegate( RandomGenerator random, Character src, Character target )
					{
						uint accuracy = (uint)(src.baseStats[(int)StatType.Dex]);
						if (target.HitCheck(random, HitType.Melee, accuracy))
						{
							uint damage = src.actualParams[(int)ParamType.phyAtk];
							uint minDamage = (uint)(damage * 0.8f);
							damage = (uint)random.Next((int)minDamage, (int)damage);
							target.Hit(HitType.Melee, damage);
						}
						return true;
					}
					)
				);

			SkillManager.table.Add(SkillId.Punch,
				new Skill(
					ActionType.PhyAttack,
					TargetType.Single,
					WeaponType.All,
					0,
					0,
					40,
                    delegate( RandomGenerator random, Character src, Character target )
					{
						uint accuracy = (uint)(src.baseStats[(int)StatType.Dex]*0.9);
						if (target.HitCheck(random, HitType.Melee, accuracy))
						{
							uint damage = src.actualParams[(int)ParamType.phyAtk];
							uint minDamage = (uint)(damage * 0.7f);
							damage = (uint)random.Next((int)minDamage, (int)damage);
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
					WeaponType.All,
					0,
					10,
					50,
                    delegate( RandomGenerator random, Character src, Character target )
					{
						uint accuracy = (uint)(src.baseStats[(int)StatType.Wis] * 0.4 + src.baseStats[(int)StatType.Int] * 0.6);
						if (target.HitCheck(random, HitType.Magical, accuracy))
						{
							uint damage = src.actualParams[(int)ParamType.magAtk];
							uint minDamage = (uint)(damage * 0.8f);
							damage = (uint)random.Next((int)minDamage, (int)damage);
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
					WeaponType.All,
					0,
					10,
					60,
                    delegate( RandomGenerator random, Character src, Character target )
					{
						uint damage = src.actualParams[(int)ParamType.magAtk] * 2;
						uint minDamage = (uint)(damage * 0.8f);
						damage = (uint)random.Next((int)minDamage, (int)damage);
						target.Recover(GaugeType.Hp, (uint)damage);
						return true;
					}
					)
				);

			SkillManager.table.Add(SkillId.Bite,
				new Skill(
					ActionType.PhyAttack,
					TargetType.Single,
					WeaponType.All,
					0,
					0,
					30,
					delegate(RandomGenerator random, Character src, Character target)
					{
						uint accuracy = (uint)(src.baseStats[(int)StatType.Dex] * 0.8);
						if (target.HitCheck(random, HitType.Melee, accuracy))
						{
							uint damage = src.actualParams[(int)ParamType.phyAtk];
							uint minDamage = (uint)(damage * 0.1f);
							damage = (uint)random.Next((int)minDamage, (int)damage);
							target.Hit(HitType.Melee, (uint)damage);
						}
						return true;
					}
					)
				);

		}
	}
}
