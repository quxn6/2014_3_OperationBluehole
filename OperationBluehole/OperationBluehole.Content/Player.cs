using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationBluehole.Content
{
    using Newtonsoft.Json;

	public class PlayerData
	{
        [JsonProperty( "playerId" )]
        public string pId;

        [JsonProperty( "name" )]
        public string name;

        [JsonProperty( "exp" )]
		public uint exp;

        [JsonProperty( "stat" )]
		public ushort[] stats;

        [JsonProperty( "skill" )]
        public List<SkillId> skills;

        [JsonProperty( "equipment" )]
        public List<ItemCode> equipments;

        [JsonProperty( "consumable" )]
		public List<ItemCode> consumables;

        [JsonProperty( "battleStyle" )]
		public BattleStyle battleStyle;

        public PlayerData()
        {

        }

		public PlayerData(
            string pId,
			string name,
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
            this.pId = pId;
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
			this.consumables = items;
			this.equipments = equipments;
			this.battleStyle = battleStyle;
		}

        public void UpdateFromPlayer( Player player )
        {
            this.exp = player.exp;
            this.stats = player.baseStats;
            this.skills = player.skills;
            this.consumables = player.items;
            this.equipments = player.equipments;
            this.battleStyle = player.battleStyle;
        }
	}

	public class Player : Character
	{
		public string pId { get; private set; }
        public uint exp { get; private set; }

		public Player()
		{
			pId = "";
			exp = 0;
		}

		public bool LoadPlayer( PlayerData data )
		{
            this.pId = data.pId;
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
			this.items = data.consumables;
			this.equipments = data.equipments;

			CalcStat();

			return true;
		}

        public bool LevelUp(ushort upLev)
        {
            ushort lev = this.baseStats[(int)StatType.Lev];
            var reqExp = GetExpToLevelUp(upLev);
            if ( this.exp >= reqExp )
            {
                exp -= reqExp;
                lev += upLev;
                this.baseStats[(int)StatType.Lev] = lev;

                // GET_SKILL_PER_LEVEL_UP 마다 랜덤으로 스킬 하나 획득
                if ((lev - 1) % Config.GET_SKILL_PER_LEVEL_UP == 0 && SkillManager.table.Count > this.skills.Count)
                {
                    int notHaveCnt = SkillManager.table.Count - this.skills.Count;
                    Random rnd = new Random();
                    notHaveCnt = rnd.Next(0, notHaveCnt-1);
                    var learn = SkillManager.table.Keys.Where(i => !this.skills.Contains(i)).Skip(notHaveCnt).First();
                    this.skills.Add(learn);
                }

                return true;
            }
            else
                return false;
        }

        public int GetBonusStats()
        {
            int totalStats = GetTotalStatsAtLevel(this.baseStats[(int)StatType.Lev]);
            int curStats = this.baseStats.Skip(1).Take(6).Sum(i => i);
            return totalStats - curStats;
        }

        // upLev만큼의 레벨 업에 필요한 경험치
        public uint GetExpToLevelUp(ushort upLev)
        {
            uint reqExp = 0;
            for (var srcLev = this.baseStats[(int)StatType.Lev]; srcLev < srcLev + upLev; ++srcLev)
            {
                reqExp += (uint)srcLev * srcLev * 10;
            }

            return reqExp;
        }

        public bool SetBonusStats(List<Tuple<StatType, ushort>> upStats)
        {
            if (upStats.Sum(i => i.Item2) > GetBonusStats())
                return false;

            upStats.ForEach(i =>
            {
                this.baseStats[(int)i.Item1] += i.Item2;
            });

            return true;
        }
        public bool SetBonusStat(StatType upStatType, ushort upStatValue)
        {
            if (upStatValue > GetBonusStats())
                return false;

            this.baseStats[(int)upStatType] += upStatValue;
            return true;
        }

        public bool CarryItems(List<ItemCode> inputItems)
        {
            if (this.items.Count + inputItems.Count > Config.MAX_CARRY_ITEMS)
                return false;

            this.items.AddRange(inputItems);
            return true;
        }

        public bool CarryItem(ItemCode inputItem)
        {
            if (this.items.Count >= Config.MAX_CARRY_ITEMS)
                return false;

            this.items.Add(inputItem);
            return true;
        }

        // 휴대중인 아이템중 뺄 목록을 요청하면, 빠진 아이템 목록이 리턴
        public List<ItemCode> UnCarryItems(List<ItemCode> outputReq)
        {
            var output = new List<ItemCode>();
            outputReq.ForEach(i =>
            {
                if (this.items.Remove(i))
                    output.Add(i);
            });
            return output;
        }
        // 휴대중인 아이템중 뺄 아이템코드를 요청하면, 실제로 해당 아이템을 소지하고 있었을시 true 리턴, 없었다면 false
        public bool UnCarryItem(ItemCode outputReq)
        {
            if (this.items.Remove(outputReq))
                return true;
            else
                return false;
        }
    }
}
