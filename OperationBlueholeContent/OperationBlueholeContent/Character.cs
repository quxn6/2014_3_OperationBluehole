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
        StatCount,  // 전체 statType 수
	}

    enum ParamType : byte
    {
        phyAtk,
        phyDef,
        magAtk,
        magDef,
        spRegn, // 하나만 튀어 나와 있어서 spRegen에서 바꿈 ㅋㅋ
        maxHp,
        maxMp,
        pramCount,  // 전체 paramType 수
    }

	enum GaugeType : byte
	{
		Hp,
		Mp,
		Sp,
	}

    class BuffPiece : IComparable<BuffPiece>
    {
        public BuffCode id;
        public uint expireTime;

        public BuffPiece( BuffCode id, uint expireTime )
        {
            this.id = id;
            this.expireTime = expireTime;
        }

        public int CompareTo( BuffPiece obj )
        {
            return this.expireTime.CompareTo( obj.expireTime );
        }
    }

    class BuffComp : Comparer<BuffPiece>
    {
        public override int Compare( BuffPiece lhs, BuffPiece rhs )
        {
            if ( object.Equals( lhs, rhs ) ) return 0;
            return lhs.expireTime.CompareTo( rhs.expireTime );
        }
    }

	class Character : GameObject
	{
        // stat = 캐릭터의 능력치
        // param = 전투에 사용되는 수치( 기본적으로 stat으로부터 유도된다 )
        // actualParams = effectParams + ( baseStats + extraStats ) * 쿵덕쿵
        public ushort[] baseStats { get; protected set; }
        public ushort[] extraStats { get; protected set; }

        public uint[] effectParams { get; protected set; }
        public uint[] actualParams { get; protected set; }

		public uint hp { get; private set; }
		public uint mp { get; private set; }
		public uint sp { get; private set; }

		public List<SkillId> skills { get; protected set; }
		public List<ItemCode> items { get; protected set; }
        public List<ItemCode> equipments { get; protected set; }
        public MinHeap<BuffPiece> buffs { get; protected set; }
		public EquipType equipStatus { get; private set; }
		//TODO: 버프리스트

		public Character()
		{
			Reset();
		}

		public void Reset()
		{
            baseStats = new ushort[(int)StatType.StatCount] { 1, 5, 5, 5, 5, 5, 5, 5 };
            extraStats = new ushort[(int)StatType.StatCount];

            effectParams = new uint[(int)ParamType.pramCount];
            actualParams = new uint[(int)ParamType.pramCount];

			skills = new List<SkillId>();
			items = new List<ItemCode>();
            equipments = new List<ItemCode>();
            buffs = new MinHeap<BuffPiece>( new BuffComp() );
		}

		public void CalcStat()
		{
            // 1. base stat 로드
                // 이미 완료 됐다 치고...

            // 2. extra stat과 effectParams은 모두 0으로 설정
            Array.Clear( extraStats, 0, extraStats.Length );
            Array.Clear( effectParams, 0, effectParams.Length );
            
            // 3. 아이템 정보 로드
                // 이것도 됐다 치고...

            // 4. 아이템 마다 효과를 extra stat과 extra param에 적용
            foreach ( var id in equipments )
            {
                var equip = (Equipment)ItemManager.table[id];

                // tank's code
                if ( ( equipStatus & equip.equipType ) > 0 )
                    UnEquipItem( id );
                else
                    equipStatus |= equip.equipType;

                // equip.action( this, this );
                // 여기까지

                equip.plusStat.ForEach( stat => extraStats[(int)stat.Item1] += stat.Item2 );
                equip.plusParam.ForEach( param => effectParams[(int)param.Item1] += param.Item2 );
            }

            // 5. 버프 리스트 로드
                // 이것도 됐다 치고...

            // 6. 버프마다의 효과를 base stat과 extra param에 적용
            foreach ( var buffPiece in buffs )
            {
                var buff = (Buff)BuffManager.table[buffPiece.id];

                buff.plusStat.ForEach( stat => extraStats[(int)stat.Item1] += stat.Item2 );
                buff.plusParam.ForEach( param => effectParams[(int)param.Item1] += param.Item2 );
            }

            // 조심해!
            // 7. Base stat과 extra stat의 합으로 1차 actual param 계산
            // 지금은 임시 값들 사용하고
            // 나중에 직업별 static function으로 만들어서 거기서 계산 할 것
            actualParams[(int)ParamType.phyAtk] = (uint)( baseStats[(int)StatType.Str] + extraStats[(int)StatType.Str] );
            actualParams[(int)ParamType.phyDef] = (uint)( baseStats[(int)StatType.Agi] + extraStats[(int)StatType.Agi] );
            actualParams[(int)ParamType.magAtk] = (uint)( baseStats[(int)StatType.Dex] + extraStats[(int)StatType.Dex] );
            actualParams[(int)ParamType.magDef] = (uint)( baseStats[(int)StatType.Int] + extraStats[(int)StatType.Int] );
            actualParams[(int)ParamType.spRegn] = (uint)( baseStats[(int)StatType.Mov] + extraStats[(int)StatType.Mov] );
            actualParams[(int)ParamType.maxHp] = (uint)( baseStats[(int)StatType.Con] + extraStats[(int)StatType.Con] ) * 20;
            actualParams[(int)ParamType.maxMp] = (uint)( baseStats[(int)StatType.Wis] + extraStats[(int)StatType.Wis] );

            // 8. effect param을 actual param에 적용
            for ( int i = 0; i < actualParams.Length; ++i )
                actualParams[i] += effectParams[i];

            // 조심해!
            // 일단 여기에서 초기값 할당은 하지만 나중에 적당한 위치로 이동하는 게 좋을 것 같아
            hp = actualParams[(int)ParamType.maxHp];
            mp = actualParams[(int)ParamType.maxHp];
            sp = 0;
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
                    if ( hp > actualParams[(int)ParamType.maxHp] )
                        hp = actualParams[(int)ParamType.maxHp];
					break;
				case GaugeType.Mp:
					mp -= value;
                    if ( mp > actualParams[(int)ParamType.maxMp] )
                        mp = actualParams[(int)ParamType.maxMp];
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
                    if ( hp > actualParams[(int)ParamType.maxHp] )
                        hp = actualParams[(int)ParamType.maxHp];
					break;
				case GaugeType.Mp:
					mp += value;
                    if ( mp > actualParams[(int)ParamType.maxMp] )
                        mp = actualParams[(int)ParamType.maxMp];
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
				if (baseStats[(int)stat.Item1] < stat.Item2)
					return false;
			}

			// 착용 상태에 추가
			equipStatus |= item.equipType;
			equipments.Add(id);

			// 추가 스탯 적용
			foreach (var stat in item.plusStat)
				baseStats[(int)stat.Item1] += stat.Item2;

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
				extraStats[(int)stat.Item1] -= stat.Item2;

			// TODO: 장비의 추가 효과 제거. 버프로 처리 예정

			return true;
		}

        public void CheckBuff( uint currentTick )
        {
            while ( buffs.Peek().expireTime < currentTick )
            {
                DeregisterBuff();
            }
        }

        public bool RegisterBuff( uint currentTick, BuffCode id )
        {
            var buff = (Buff)BuffManager.table[id];

            if ( buff == null )
                return false;

            buffs.Push( new BuffPiece( id, buff.duration + currentTick ) );

            buff.plusStat.ForEach( stat => extraStats[(int)stat.Item1] += stat.Item2 );
            buff.plusParam.ForEach( param => effectParams[(int)param.Item1] += param.Item2 );

            // 바뀌었으니 다시 계산
            CalcStat();

            return true;
        }

        public bool DeregisterBuff()
        {
            var buff = (Buff)BuffManager.table[buffs.Peek().id];

            buff.plusStat.ForEach( stat => extraStats[(int)stat.Item1] -= stat.Item2 );
            buff.plusParam.ForEach( param => effectParams[(int)param.Item1] -= param.Item2 );

            buffs.Pop();

            // 바뀌었으니 다시 계산
            CalcStat();

            return true;
        }
	}

	struct PlayerData
	{
		public uint exp;
		public ushort[] stats;

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
