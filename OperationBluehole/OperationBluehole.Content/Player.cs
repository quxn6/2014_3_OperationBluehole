using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationBluehole.Content
{
	struct PlayerData
	{
		public String name;
		public uint exp;
		public ushort[] stats;

		public List<SkillId> skills;
		public List<ItemCode> items;
		public List<ItemCode> equipments;
		public BattleStyle battleStyle;

		public PlayerData(
			String name,
			uint exp,
			ushort statLev,
			ushort statStr,
			ushort statDex,
			ushort statInt,
			ushort statCon,
			ushort statAgi,
			ushort statWis,
			ushort statMov,
			List<SkillId> skills,
			List<ItemCode> items,
			List<ItemCode> equipments,
			BattleStyle battleStyle
			)
		{
			this.name = name;
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
			this.skills = skills;
			this.items = items;
			this.equipments = equipments;
			this.battleStyle = battleStyle;
		}
	}

	public class Player : Character
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
			if (!TestData.playerList.TryGetValue(playerId, out data))
				return false;

			this.name = data.name;
			this.exp = data.exp;
			this.baseStats[(int)StatType.Lev] = data.stats[(int)StatType.Lev];
			this.baseStats[(int)StatType.Str] = data.stats[(int)StatType.Str];
			this.baseStats[(int)StatType.Dex] = data.stats[(int)StatType.Dex];
			this.baseStats[(int)StatType.Int] = data.stats[(int)StatType.Int];
			this.baseStats[(int)StatType.Con] = data.stats[(int)StatType.Con];
			this.baseStats[(int)StatType.Agi] = data.stats[(int)StatType.Agi];
			this.baseStats[(int)StatType.Wis] = data.stats[(int)StatType.Wis];
			this.baseStats[(int)StatType.Mov] = data.stats[(int)StatType.Mov];

			this.skills = data.skills;
			this.items = data.items;
			this.equipments = data.equipments;

			CalcStat();

			return true;
		}
	}
}
