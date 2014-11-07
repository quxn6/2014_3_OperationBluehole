using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
	enum MobId : uint
	{
		Dummy	= 0,
		Spider_Small,
	}

	class Mob : Character
	{
		// 보상 리스트가 있어야 한다
		// 경험치와 골드와 아이템(지금은 일단 토큰만 준다)
		public readonly int rewardExp;
		public readonly int rewardGold;
		public readonly Item rewardItem;

		public Mob(MobData data)
		{
			this.baseStats = data.stats;
			this.skills = data.skills;
			this.items = data.items;
			this.equipments = data.equipments;
			this.battleStyle = data.battleStyle;
			this.rewardExp = data.rewardExp;
			this.rewardGold = data.rewardGold;
			this.rewardItem = data.rewardItem;

			CalcStat();
		}
	}

	struct MobData
	{
		public String name;
		public ushort[] stats;
		public List<SkillId> skills;
		public List<ItemCode> items;
		public List<ItemCode> equipments;
		public BattleStyle battleStyle;
		public int rewardExp;
		public int rewardGold;
		public Item rewardItem;
	}

	struct MobTypeData
	{
		public String name;
		public ushort[] statRate; // 스탯 분배 비율
		public ushort statMov;
		public List<SkillId> skills;
		public List<ItemCode> items;
		public List<ItemCode> equipments;
		public BattleStyle[] battleStyles;
		public int tokenDropChance; // milli percent

		public MobTypeData(
			String name,
			ushort strRate,
			ushort dexRate,
			ushort intRate,
			ushort conRate,
			ushort agiRate,
			ushort wisRate,
			ushort statMov,
			List<SkillId> skills,
			List<ItemCode> items,
			List<ItemCode> equipments,
			BattleStyle[] battleStyles,
			int tokenDropChance
			)
		{
			this.name = name;
			this.statRate = new ushort[] { strRate, dexRate, intRate, conRate, agiRate, wisRate };
			this.statMov = statMov;
			this.skills = skills;
			this.items = items;
			this.equipments = equipments;
			this.battleStyles = battleStyles;
			this.tokenDropChance = tokenDropChance;
		}
	}

	static class MobGenerator
	{
		static Dictionary<MobId, MobTypeData> mobTypeDataTable;
		public static MobData GetMobData(RandomGenerator random, MobId mobId, ushort level)
		{
			MobData newData = new MobData();

			newData.name = mobTypeDataTable[mobId].name;

			// 스탯 statRate에 따라 랜덤 분배
			ushort[] stats = new ushort[(int)StatType.StatCount];
			{
				stats[(int)StatType.Lev] = level;
				
				int totalStat = level * 3 + 27;
				ushort[] statRate = mobTypeDataTable[mobId].statRate;

				int[] statBlock = new int[statRate.Sum(i => i) + 1];
				statBlock[0] = 0;
				for (int i = 1; i < statBlock.Length - 1; ++i)
				{
					int tmp;
					do
					{
						tmp = random.Next(1, totalStat - 1);
					} while (statBlock.Contains(tmp));
					statBlock[i] = tmp;
				}
				statBlock[statBlock.Length - 1] = totalStat;
				statBlock = statBlock.OrderBy(i => i).ToArray();

				int cnt = 0;
				for (int i = (int)StatType.Str; i < (int)StatType.Mov; ++i)
				{
					stats[i] = (ushort)(statBlock[cnt + statRate[i-1]] - statBlock[cnt]);
					cnt += statRate[i-1];
				}
				
				stats[(int)StatType.Mov] = mobTypeDataTable[mobId].statMov;
			}
			newData.stats = stats;

			newData.skills = mobTypeDataTable[mobId].skills;
			newData.items = mobTypeDataTable[mobId].items;
			newData.equipments = mobTypeDataTable[mobId].equipments;
			newData.battleStyle = mobTypeDataTable[mobId].battleStyles[random.Next(mobTypeDataTable[mobId].battleStyles.Length-1)];

			newData.rewardExp = level * Config.MOB_REWARD_EXP_WEIGHT;
			newData.rewardGold = level * Config.MOB_REWARD_GOLD_WEIGHT;

			if (random.Next(Config.MOB_REWARD_ITEM_CHANCE_MAX) < mobTypeDataTable[mobId].tokenDropChance)
				newData.rewardItem = new ItemToken(
					level + random.Next(
						(int)(-level * Config.MOB_REWARD_ITEM_RANGE),
						(int)(level * Config.MOB_REWARD_ITEM_RANGE)
					), random);
			else
				newData.rewardItem = null;

			return newData;
		}

		public static void Init()
		{
			mobTypeDataTable = new Dictionary<MobId, MobTypeData>();
			// Spider_Small
			{
				MobTypeData tData = new MobTypeData(
					"SmallSpider",
					1, 2, 1, 1, 1, 1,
					5,
					new List<SkillId>() 
					{
						SkillId.Bite 
					},
					new List<ItemCode>()
					{
					},
					new List<ItemCode>()
					{
					},
					new BattleStyle[] { 
						BattleStyle.AGGRESSIVE
					},
					5000
					);
				MobGenerator.mobTypeDataTable.Add(MobId.Spider_Small,
					tData
					);
			}


		}
	}
}
