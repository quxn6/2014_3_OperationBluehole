using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// raw data for map
public struct Dungeon
{
	public char[] dungeonMap;
	public int size;

	public Dungeon( string dungeonMap , int size )
	{
		this.dungeonMap = dungeonMap.ToCharArray();
		this.size = size;
	}
}

public struct CharacterData
{
	public float hp;
	public float mp;
	public float sp;

	public CharacterData( float hp , float mp , float sp )
	{
		this.hp = hp;
		this.mp = mp;
		this.sp = sp;
	}
}

public struct EnemyGroup
{
	public CharacterData[] enemies;
	public EnemyGroup( int mobCount , CharacterData mob )
	{
		this.enemies = new CharacterData[mobCount];
		for ( int i = 0 ; i < mobCount ; ++i )
		{
			enemies[i] = mob;
		}
	}

	public EnemyGroup( EnemyGroup otherEnemyGroup )
	{
		int len = otherEnemyGroup.enemies.Length;
		this.enemies = new CharacterData[len];
		for ( int i = 0 ; i < len ; ++i )
		{
			enemies[i] = otherEnemyGroup.enemies[i];
		}
	}
}

static public class MobType
{
	static public CharacterData spider = new CharacterData(
		GameConfig.MOB_SPIDER_HP ,
		GameConfig.MOB_SPIDER_MP ,
		GameConfig.MOB_SPIDER_SP );

	static public EnemyGroup spiderGroup = new EnemyGroup( GameConfig.MOB_SPIDER_GROUP_COUNT , spider );
}


// manage raw data that had received from server in json form
public class DataManager : MonoBehaviour
{
	private List<EnemyGroup> enemyGroupList;
	public List<EnemyGroup> EnemyGroupList
	{
		get { return enemyGroupList; }
	}

	private List<CharacterData> heroList;
	public List<CharacterData> HeroList
	{
		get { return heroList; }
	}

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
		enemyGroupList = new List<EnemyGroup>();
		heroList = new List<CharacterData>();
	}

	// Get data from Logic( or server )
	public void InitEnemyDataList()
	{
		// Warning!!! It's Generate dummy data now		
		for ( int i = 0 ; i < GameConfig.DUMMY_ENEMY_COUNT ; ++i )
		{
			enemyGroupList[i] = new EnemyGroup( MobType.spiderGroup );
		}
	}

	// It have to contains 4 hero, 
	public void InitUserDataList()
	{
		heroList.Add( new CharacterData( 100.0f , 100.0f , 30.0f ) );
		heroList.Add( new CharacterData( 200.0f , 100.0f , 30.0f ) );
		heroList.Add( new CharacterData( 300.0f , 50.0f , 20.0f ) );
		heroList.Add( new CharacterData( 200.0f , 300.0f , 50.0f ) );
	}
}
