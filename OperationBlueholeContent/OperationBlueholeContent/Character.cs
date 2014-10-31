using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace OperationBlueholeContent
{
	// 전투 성향, 차후 AI행동시 사용...되려나
	enum BattleStyle : byte
	{
		AGGRESSIVE,
		DEFENSIVE,
		SUPPORTIVE,
	}
	// 플레이 성향
	enum PlayStyle : byte
	{
		SELFISH,
		NEUTRAL,
		SELFLESS,
	}

	enum HitType : byte
	{
		Melee,
		Range,
		Magical,
	}

	enum StatType : byte
	{
		Lev,
		Str,
		Dex,
		Int,
		Con,
		Agi,
		Wis,
		Mov,
	}

	enum GaugeType : byte
	{
		Hp,
		Mp,
		Sp,
	}

	class Character : GameObject
	{
		public ushort[] stats { get; protected set; }

		public uint statHp { get; protected set; }
		public uint statMp { get; protected set; }

		public uint hp { get; private set; }
		public uint mp { get; private set; }
		public uint sp { get; private set; }
		
		// 장비로 인한 물리/마법 공격력/방어력
		public uint phyAtk { get; private set; }
		public uint phyDef { get; private set; }
		public uint magAtk { get; private set; }
		public uint magDef { get; private set; }

		public List<SkillId> skills { get; protected set; }
		public List<ItemCode> items { get; protected set; }
		public List<ItemCode> equipments { get; protected set; }
		public EquipType equipStatus { get; private set; }
		//TODO: 버프리스트

		public Character()
		{
			Reset();
		}

		public void Reset()
		{
			stats = new ushort[8] { 1, 5, 5, 5, 5, 5, 5, 5 };

			hp = statHp = 100;
			mp = statMp = 100;
			sp = 0;

			phyAtk = phyDef = magAtk = magDef = 0;

			skills = new List<SkillId>();
			items = new List<ItemCode>();
			equipments = new List<ItemCode>();
		}

		public void CalcStat()
		{
			phyAtk = phyDef = magAtk = magDef = 0;
			foreach (var iid in equipments)
			{
				var item = (Equipment)ItemManager.table[iid];
				
				if ((equipStatus & item.equipType) > 0)
					UnEquipItem(iid);
				else
					equipStatus |= item.equipType;

				phyAtk += item.phyAtk;
				phyDef += item.phyDef;
				magAtk += item.magAtk;
				magDef += item.magDef;
// 				item.action(this, this);
			}
		}

		public bool ReduceForAction(uint hpNeed, uint mpNeed, uint spNeed)
		{
			if (hpNeed > hp || mpNeed > mp || spNeed > sp)
				return false;

			hp -= hpNeed;
			mp -= mpNeed;
			sp -= spNeed;
			return true;
		}

		public bool ReduceItem(ItemCode usedItem)
		{
			return items.Remove(usedItem);
		}

		// 차후 적과 아군의 파티 정보를 보고 행동을 결정하는 AI 추가 필요
		// 현재는 그냥 가장 체력 낮은 적부터 다굴...
		public void BattleTurnAction(Random random, Party ally, Party enemy)
		{
			if (ally == null || enemy == null)
				return;

			SkillId sid;
			Character weakAlly = ally.characters
				.Where(c => c.hp > 0 && c.hp < 50)
				.OrderBy(c => c.hp)
				.FirstOrDefault();
			if (weakAlly != null)
			{
				sid = skills.Where(id => SkillManager.table[id].type == SkillType.Heal).FirstOrDefault();
				if (sid == SkillId.None)
				{
					ItemCode iid = items.Where(code => code == ItemCode.HpPotionS).FirstOrDefault();
					if (iid != ItemCode.None)
					{
						Consumable item = (Consumable)ItemManager.table[iid];
						item.UseItem(random, this, weakAlly);
					}
				}
				else if (SkillManager.table[sid].spNeed <= sp && SkillManager.table[sid].mpNeed <= mp)
					SkillManager.table[sid].Act(random, this, weakAlly);
			}

			Character weakEnemy = enemy.characters.Where(c => c.hp > 0).OrderBy(c => c.hp).FirstOrDefault();
			sid = 0;
			sid = skills
				.Where(id => SkillManager.table[id].type == SkillType.Attack)
				.OrderByDescending(id => SkillManager.table[id].mpNeed).FirstOrDefault();
			if (sid != SkillId.None && SkillManager.table[sid].spNeed <= sp && SkillManager.table[sid].mpNeed <= mp)
				SkillManager.table[sid].Act(random, this, weakEnemy);
		}

		public bool HitCheck(HitType type, uint accuracy)
		{
			//TODO: 명중률과 회피율 계산하여 명중 여부 판단.
			return true;
		}
		public void Hit(HitType type, uint damage)
		{
			//TODO: 방어력 계산하여 실제 피해량 적용
			Damage(GaugeType.Hp, damage);
		}

		public void Damage(GaugeType type, uint value)
		{
			switch (type)
			{
				case GaugeType.Hp:
					hp -= value;
					if (hp > statHp)
						hp = statHp;
					break;
				case GaugeType.Mp:
					mp -= value;
					if (mp > statMp)
						mp = statMp;
					break;
				case GaugeType.Sp:
					sp -= value;
					if (sp > 100)
						sp = 100;
					break;
			}
		}

		public void Recover(GaugeType type, uint value)
		{
			switch(type)
			{
				case GaugeType.Hp:
					hp += value;
					if (hp > statHp)
						hp = statHp;
					break;
				case GaugeType.Mp:
					mp += value;
					if (mp > statMp)
						mp = statMp;
					break;
				case GaugeType.Sp:
					sp += value;
					if (sp > 100)
						sp = 100;
					break;
			}
		}

		public bool EquipItem(ItemCode id)
		{
			if (ItemManager.table[id].type != ItemType.Equip)
				return false;

			Equipment item = (Equipment)ItemManager.table[id];
			if ((equipStatus & item.equipType) > 0)
				return false;
			
			// 장비 요구 스탯 확인
			foreach (var stat in item.reqStat)
			{
				if (stats[(int)stat.Item1] < stat.Item2)
					return false;
			}

			// 착용 상태에 추가
			equipStatus |= item.equipType;
			equipments.Add(id);

			// 추가 스탯 적용
			foreach (var stat in item.plusStat)
				stats[(int)stat.Item1] += stat.Item2;

			// TODO: 장비의 추가 효과 적용. 버프로 처리 예정
// 			if (item.action != null)
// 				item.action(this, this);

			return true;
		}

		public bool UnEquipItem(ItemCode id)
		{
			if (!this.equipments.Contains(id))
				return false;

			Equipment item = (Equipment)ItemManager.table[id];

			// 착용 상태에서 제거
			equipStatus -= item.equipType;
			equipments.Remove(id);

			// 추가 스탯 제거
			foreach (var stat in item.plusStat)
				stats[(int)stat.Item1] -= stat.Item2;

			// TODO: 장비의 추가 효과 제거. 버프로 처리 예정

			return true;
		}
	}

	struct PlayerData
	{
		public uint exp;
		public ushort[] stats;

		public uint statHp;
		public uint statMp;

		public List<SkillId> skills;
		public List<ItemCode> items;
		public List<ItemCode> equipments;

		public PlayerData(
			uint exp,
			ushort statLev,
			ushort statStr,
			ushort statDex,
			ushort statInt,
			ushort statCon,
			ushort statAgi,
			ushort statWis,
			ushort statMov,
			uint statHp,
			uint statMp,
			List<SkillId> skills,
			List<ItemCode> items,
			List<ItemCode> equipments
			)
		{
			this.exp = exp;
			this.stats = new ushort[8];
			this.stats[(int)StatType.Lev] = statLev;
			this.stats[(int)StatType.Str] = statStr;
			this.stats[(int)StatType.Dex] = statDex;
			this.stats[(int)StatType.Int] = statInt;
			this.stats[(int)StatType.Con] = statCon;
			this.stats[(int)StatType.Agi] = statAgi;
			this.stats[(int)StatType.Wis] = statWis;
			this.stats[(int)StatType.Mov] = statMov;
			this.statHp = statHp;
			this.statMp = statMp;
			this.skills = skills;
			this.items = items;
			this.equipments = equipments;
		}
	}

	class Player : Character
	{
		public ulong pId { get; private set; }
		public uint exp { get; private set; }

		public Player()
		{
			pId = 0;
			exp = 0;
		}

		public bool LoadPlayer(ulong playerId)
		{
			PlayerData data;
			if( !TestData.playerList.TryGetValue(playerId, out data) )
				return false;

			this.exp = data.exp;
			this.stats[(int)StatType.Lev] = data.stats[(int)StatType.Lev];
			this.stats[(int)StatType.Str] = data.stats[(int)StatType.Str];
			this.stats[(int)StatType.Dex] = data.stats[(int)StatType.Dex];
			this.stats[(int)StatType.Int] = data.stats[(int)StatType.Int];
			this.stats[(int)StatType.Con] = data.stats[(int)StatType.Con];
			this.stats[(int)StatType.Agi] = data.stats[(int)StatType.Agi];
			this.stats[(int)StatType.Wis] = data.stats[(int)StatType.Wis];
			this.stats[(int)StatType.Mov] = data.stats[(int)StatType.Mov];

			this.statHp = data.statHp;
			this.statMp = data.statMp;

			this.skills = data.skills;
			this.items = data.items;
			this.equipments = data.equipments;

			return true;
		}
	}

	// 몹에는 드랍 아이템, 경험치 같은게 들어가야하려나
	class Mob : Character
	{
		public Mob()
		{
			//for test
			skills.Add(SkillId.Punch);
		}
	}
}
