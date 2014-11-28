﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct CharacterData
{
	public float maxHp;
	public float maxMp;
	public float currentHp;
	public float currentMp;
	public float sp;
	public float spRegen;
	public int level;

	public CharacterData( float hp , float mp , float sp , float spRegen, int level)
	{
		this.maxHp = hp;
		this.maxMp = mp;
		this.currentHp = hp;
		this.currentMp = mp;
		this.sp = sp;
		this.spRegen = spRegen;
		this.level = level;
	}
}
// 
// public struct EnemyData
// {
// 	public CharacterData characterData;
// 	public MobType mobType;
// }
// 
// public struct EnemyGroup
// {
// 	public EnemyData[] enemies;
// 	public EnemyGroup( int mobCount , CharacterData cd , MobType mt )
// 	{
// 		this.enemies = new EnemyData[mobCount];
// 		for ( int i = 0 ; i < mobCount ; ++i )
// 		{
// 			enemies[i].characterData = cd;
// 			enemies[i].mobType = mt;
// 		}
// 	}
// 
// 	public EnemyGroup( EnemyGroup otherEnemyGroup )
// 	{
// 		int len = otherEnemyGroup.enemies.Length;
// 		this.enemies = new EnemyData[len];
// 		for ( int i = 0 ; i < len ; ++i )
// 		{
// 			enemies[i] = otherEnemyGroup.enemies[i];
// 		}
// 	}
// }

// static public class MobTypeTemp
// {
// 	static public CharacterData dummyMob = new CharacterData(
// 		GameConfig.MOB_DUMMY_HP ,
// 		GameConfig.MOB_DUMMY_MP ,
// 		GameConfig.MOB_DUMMY_SP ,
// 		3 );
// 
// 	static public EnemyGroup spiderGroup = new EnemyGroup( GameConfig.MOB_SPIDER_GROUP_COUNT , dummyMob , (MobType)( Random.Range( 1 , 10 ) ) );
// }


// manage raw data that had received from server in json form
public class DataManager : MonoBehaviour
{
	public UIAtlas atlasSet = null;

	// real data
	public OperationBluehole.Content.Party UserParty { get; private set; }
	public List<OperationBluehole.Content.Party> MobPartyList{ get; private set; }
	public List<OperationBluehole.Content.Item> ItemList{ get; private set; }

	public List<OperationBluehole.Content.Party> EncounteredMobPartyList { get; set; }
	public List<OperationBluehole.Content.Item> LootedItemList { get; set; }

// 	private List<EnemyGroup> enemyGroupList;
// 	public List<EnemyGroup> EnemyGroupList
// 	{
// 		get { return enemyGroupList; }
// 	}
// 
// 	private List<CharacterData> heroList;
// 	public List<CharacterData> HeroList
// 	{
// 		get { return heroList; }
// 	}

	static private DataManager instance = null;
	static public DataManager Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		if ( null != instance )
		{
			Debug.LogError( this + " already exist" );
			return;
		}

		instance = this;

// 		enemyGroupList = new List<EnemyGroup>();
// 		heroList = new List<CharacterData>();
		EncounteredMobPartyList = new List<OperationBluehole.Content.Party>();
	}

	public void SetReplayMapData(OperationBluehole.Content.Party userParty, List<OperationBluehole.Content.Party> mobPartyList, List<OperationBluehole.Content.Item> itemList)
	{
		this.UserParty = userParty;
		this.MobPartyList = mobPartyList;
		this.ItemList = itemList;
	}
	
// 	// Get data from Logic( or server )
// 	public void InitEnemyDataList()
// 	{
// 		// Warning!!! It's Generate dummy data now		
// 		for ( int i = 0 ; i < GameConfig.DUMMY_ENEMY_COUNT ; ++i )
// 		{
// 			enemyGroupList.Add( 
// 				new EnemyGroup( 
// 					GameConfig.MOB_SPIDER_GROUP_COUNT , 
// 					new CharacterData(		
// 						GameConfig.MOB_DUMMY_HP ,
// 						GameConfig.MOB_DUMMY_MP ,
// 						GameConfig.MOB_DUMMY_SP , 
// 						Random.Range(1,11) ) , 
// 					(MobType)( Random.Range( 1 , (int)MobType.Length ) ) 
// 				) 
// 			);
// 		}
// 	}

// 	// It have to contains 4 hero, 
// 	public void InitUserDataList()
// 	{
// 		heroList.Add( new CharacterData( 100.0f , 100.0f , 30.0f , 1 ) );
// 		heroList.Add( new CharacterData( 200.0f , 100.0f , 30.0f , 2 ) );
// 		heroList.Add( new CharacterData( 300.0f , 50.0f , 20.0f , 10 ) );
// 		heroList.Add( new CharacterData( 200.0f , 300.0f , 50.0f , 3 ) );
// 	}
}
