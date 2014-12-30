using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OperationBluehole.Content
{
	static class TestData
	{
		public static Dictionary<ulong, PlayerData> playerList { get; private set; }

		public static void InitPlayer()
		{
			TestData.playerList = new Dictionary<ulong, PlayerData>();

			TestData.playerList.Add(101, new PlayerData
            (
                "TestPlayer101",
                "TestPlayer101",
				0,
				1,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				new List<SkillId>{
					SkillId.Punch
				},
				new List<ItemCode>
				{

				},
				new List<ItemCode>
				{

				},
				BattleStyle.AGGRESSIVE
			));

			TestData.playerList.Add(102, new PlayerData
			(
                "TestPlayer102",
                "TestPlayer102",
				0,
				3,
				5,
				5,
				5,
				15,
				5,
				5,
				5,
				new List<SkillId>{
					SkillId.Punch,
					SkillId.MagicArrow,
					SkillId.Heal,
				},
				new List<ItemCode>
				{
					ItemCode.MpPotion_S,
				},
				new List<ItemCode>
				{

				},
				BattleStyle.DEFENSIVE
			));

			TestData.playerList.Add(103, new PlayerData
			(
                "TestPlayer103",
                "TestPlayer103",
				0,
				3,
				15,
				5,
				5,
				5,
				5,
				5,
				5,
				new List<SkillId>{
					SkillId.Slash,
				},
				new List<ItemCode>
				{
					ItemCode.HpPotion_S,
					ItemCode.HpPotion_S,
				},
				new List<ItemCode>
				{
					ItemCode.Sword_Test,
				},
				BattleStyle.AGGRESSIVE
			));

			TestData.playerList.Add(104, new PlayerData
			(
                "TestPlayer104",
                "TestPlayer104",
				0,
				3,
				10,
				10,
				5,
				5,
				5,
				5,
				5,
				new List<SkillId>{
					SkillId.Punch,
					SkillId.Heal,
				},
				new List<ItemCode>
				{
					ItemCode.MpPotion_S,
				},
				new List<ItemCode>
				{

				},
				BattleStyle.AGGRESSIVE
			));

			TestData.playerList.Add( 105, new PlayerData
					{
						pId = "DefaultTestPlayer",
						name = "DefaultTestPlayer",
						exp = 0,
						stats = new ushort[] { 1, 5, 5, 5, 5, 5, 5, 5, },
						skills = new List<SkillId> { SkillId.Punch, },
						consumables = new List<ItemCode> { ItemCode.HpPotion_S, },
						equipments = new List<ItemCode> { ItemCode.Sword_Test, },
						battleStyle = BattleStyle.AGGRESSIVE,
					} 
			);
		}
	}
}
