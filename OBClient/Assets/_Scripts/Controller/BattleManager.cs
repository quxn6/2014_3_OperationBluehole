using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
	public GameObject enemyPrefab = null;
	public GameObject[] heroStatus = null;
	public GameObject playerPosition = null;
	public GameObject[] mobPositions = null;

	private GameObject[] enemyList = null;
	private List<GameObject> heroList = null;

	static private bool isInitialized = false;
	
	static private BattleManager instance;
	static public BattleManager Instance
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
	}

	// Create object pool for enemy
	public void InitBattleObjects()
	{
		// Create object pool in first battle
		if ( isInitialized )
			return;

		// Create object pool for battle area
		LgsObjectPoolManager.Instance.CreateObjectPool( enemyPrefab.name , enemyPrefab , 10 );

		// Set initilize flag
		isInitialized = true;
	}

	// Set Camera, enemy and heroes status UI
	public void PutBattleObjects( int mobId )
	{
		LoadEnemyData( mobId );
		LoadHeroData();
	}

	// Instantiate enemy object, set status and position in battle area. 
	private void LoadEnemyData(int mobId)
	{
		// Load enemy count and check validation
		int enemyCount = DataManager.Instance.EnemyGroupList[mobId].enemies.Length;
		if ( enemyCount > mobPositions.Length )
		{
			Debug.LogError( "Error(battle area) : Two many enemies loaded. current " + enemyCount + ", it have to be LE " + mobPositions.Length );
		}

		// Set the script in enemy instance
		enemyList = new GameObject[enemyCount];
		for ( int i = 0 ; i < enemyCount ; ++i )
		{
			// Instance enemy and set the status data
			enemyList[i] = LgsObjectPoolManager.Instance.ObjectPools[enemyPrefab.name].PullObject();
			enemyList[i].GetComponent<Enemy>().EnemyStat = DataManager.Instance.EnemyGroupList[mobId].enemies[i];

			// Put enemy own position
			enemyList[i].transform.parent = mobPositions[i].transform;
			enemyList[i].transform.localPosition = Vector3.zero;
			enemyList[i].transform.localEulerAngles = Vector3.zero;
		}
	}

	// Load hero status and set camera in battle area
	private void LoadHeroData()
	{
		// Check hero count validation
		int heroCount = DataManager.Instance.HeroList.Count;
		if ( heroCount != GameConfig.MAXIMUM_HERO_COUNT )
			Debug.LogError( "Error(battle area) : We need " + GameConfig.MAXIMUM_HERO_COUNT + " heroes" );

		// Set status of heroes and set UI values
		for ( int i = 0 ; i< heroCount; ++i)
		{
			heroList[i].GetComponent<Hero>().HeroStat = DataManager.Instance.HeroList[i];
			heroList[i].GetComponent<Hero>().InitHeroUI();
		}

		// Move camera on predetermined position in battle area
		EnvironmentManager.Instance.PutCamera(playerPosition, CameraMode.FIRST_PERSON);
	}

	// Clear battle area and turn back main camera to dungeon
	public void EndBattle()
	{
		LgsObjectPoolManager.Instance.ObjectPools[enemyPrefab.name].ResetPool();
		EnvironmentManager.Instance.PutCamera( MapManager.Instance.PlayerParty , CameraMode.THIRD_PERSON );
	}
}
