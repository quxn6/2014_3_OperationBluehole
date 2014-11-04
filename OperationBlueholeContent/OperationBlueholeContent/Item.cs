using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBlueholeContent
{
	enum ItemCode : uint
	{
		None = 0x0,
		Ring,
		HpPotionS,
		MpPotionS,
	}
	enum ItemCatag : ushort
	{
		None = 0x0,
		Equip = 0x1,	// 장비 가능
		Consume = 0x2,	// 소모품
	}
	enum EquipType : ushort
	{
		Head = 0x1,
		Body = 0x2,
		LHand = 0x4,
		RHand = 0x8,
		Leg = 0x10,
		Feet = 0x20,
	}

	internal class Item : GameObject
	{
		public ItemCode code { get; private set; }
		public ItemCatag catagory { get; private set; }
        public Func<RandomGenerator, Character, Character, bool> action { get; protected set; }

		public Item()
		{
			code = ItemCode.None;
			catagory = ItemCatag.None;
			action = null;
		}

		public Item(ItemCode code, ItemCatag type,
            Func<RandomGenerator, Character, Character, bool> action )
		{
			this.code = code;
			this.catagory = type;

			this.action = action;
		}
	}

	internal class Equipment : Item
	{
		public EquipType equipType { get; private set; }
		public List<Tuple<StatType, ushort>> reqStat { get; private set; }
        public List<Tuple<StatType, ushort>> plusStat { get; private set; }
        public List<Tuple<StatType, uint>> plusParam { get; private set; }
		// 		private Func<Character, Character, bool> action;

		public Equipment(ItemCode id, ItemCatag type,
			EquipType equipType,
			List<Tuple<StatType, ushort>> reqStat,
            List<Tuple<StatType, ushort>> plusStat,
            List<Tuple<StatType, uint>> plusParam,
            Func<RandomGenerator, Character, Character, bool> action )
			: base( id, type, action )
		{
			this.equipType = equipType;
			this.reqStat = reqStat;
			this.plusStat = plusStat;
            this.plusParam = plusParam;
		}
	}

	internal class Consumable : Item
	{
		public uint spNeed { get; private set; }
		public ActionType type { get; private set; }
		public TargetType targetType { get; private set; }
		// 		private Func<Character, Character, bool> action;

		public Consumable(ItemCode id, ItemCatag type,
			TargetType targetType,
			uint spNeed,
			ActionType actType,
            Func<RandomGenerator, Character, Character, bool> action )
			: base(id, type, action)
		{
			this.spNeed = spNeed;
			this.type = actType;
			this.targetType = targetType;
		}

        public bool UseItem( RandomGenerator random, Character src )
		{
			if (action == null)
				return false;
			if (targetType != TargetType.None && targetType != TargetType.All)
				return false;

			if (src.items.Contains(this.code) &&
				src.ReduceForAction(0, 0, spNeed) &&
				src.ReduceItem(this.code))
				return action(random, src, null);

			return false;
		}

        public bool UseItem( RandomGenerator random, Character src, Character target )
		{
			if (action == null)
				return false;
			if (target == null)
				return UseItem(random, src);
			if (targetType != TargetType.Single)
				return false;

			if (src.items.Contains(this.code) &&
				src.ReduceForAction(0, 0, spNeed) &&
				src.ReduceItem(this.code))
				return action(random, src, target);

			return false;
		}

        public bool UseItem( RandomGenerator random, Character src, Character[] targets )
		{
			if (action == null)
				return false;

			if (src.items.Contains(this.code) &&
				src.ReduceForAction(0, 0, spNeed) &&
				src.ReduceItem(this.code))
			{
				foreach (Character target in targets)
					action(random, src, target);
				return true;
			}

			return false;
		}
	}

	// 이걸 획득하면 게임 종료
	class RingOfErrethAkbe : Item
	{
		public RingOfErrethAkbe()
			: base(ItemCode.Ring, ItemCatag.None, null)
		{ }
	}

	static class ItemManager
	{
		public static Dictionary<ItemCode, Item> table { get; private set; }

		public static void Init()
		{
			ItemManager.table = new Dictionary<ItemCode, Item>();

			ItemManager.table.Add(ItemCode.HpPotionS,
				new Consumable(ItemCode.HpPotionS, ItemCatag.Consume, TargetType.Single,
					50,
					ActionType.RecoverHp,
                    delegate( RandomGenerator random, Character src, Character target )
					{
						target.Recover(GaugeType.Hp, 50);
						return true;
					}
				));

			ItemManager.table.Add(ItemCode.MpPotionS,
				new Consumable(ItemCode.MpPotionS, ItemCatag.Consume, TargetType.Single,
					50,
					ActionType.RecoverMp,
                    delegate( RandomGenerator random, Character src, Character target )
					{
						target.Recover(GaugeType.Mp, 20);
						return true;
					}
				));

		}
	}
}