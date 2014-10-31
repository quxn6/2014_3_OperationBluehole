using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
	static class TestData
	{
		public static Dictionary<ulong, PlayerData> playerList { get; private set; }

		public static void InitPlayer()
		{
			TestData.playerList = new Dictionary<ulong, PlayerData>();

			TestData.playerList.Add(101, new PlayerData
			(
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

				}
			));

			TestData.playerList.Add(102, new PlayerData
			(
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
					ItemCode.MpPotionS,
				},
				new List<ItemCode>
				{

				}
			));

			TestData.playerList.Add(103, new PlayerData
			(
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
					ItemCode.HpPotionS,
					ItemCode.HpPotionS,
				},
				new List<ItemCode>
				{

				}
			));

			TestData.playerList.Add(104, new PlayerData
			(
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
					ItemCode.MpPotionS,
				},
				new List<ItemCode>
				{

				}
			));
		}
	}
	class Test
	{
	}
}
