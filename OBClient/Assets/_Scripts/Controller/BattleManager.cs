﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
	public GameObject battleUI = null;
	public GameObject mobHpBarUI = null;

	public GameObject playerPosition = null;
	public GameObject[] mobPositions = null;
	public GameObject[] heroStatus = null;
	public GameObject[] mobHpBar = null;

	private GameObject[] enemyInstanceList;
	public GameObject[] EnemyInstanceList
	{
		get { return enemyInstanceList; }
		set { enemyInstanceList = value; }
	}

	private EnemyGroup enemyGroupData;

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

	// Move Camera, enemy and heroes status UI
	public void StartBattle( int targetMobId )
	{
		InitBattleObjects( targetMobId );
		LoadEnemyData();
		LoadHeroData();
		battleUI.SetActive( true );
		mobHpBarUI.SetActive( true );
	}

	// Clear battle area and turn back main camera to dungeon
	public void EndBattle()
	{
		//LgsObjectPoolManager.Instance.ObjectPools[enemyPrefab.name].ResetPool();
		for ( int i = 0 ; i < enemyGroupData.enemies.Length ; ++i )
		{
			//Destroy( enemyInstanceList[i].GetComponent<Enemy>() );
			LgsObjectPoolManager.Instance.ObjectPools[( enemyGroupData.enemies[i].mobType ).ToString()]
				.PushObject( enemyInstanceList[i] );

			// deactivate mob HP Bar
			mobHpBar[i].SetActive( false );
		}

		// Battle Area UI deactivate
		battleUI.SetActive( false );
		mobHpBarUI.SetActive( false );

		// move camera to dungeon
		EnvironmentManager.Instance.PutCamera( MapManager.Instance.PlayerParty , CameraMode.THIRD_PERSON );
	}

	// Get Enemy Group Data from data manager class
	private void InitBattleObjects( int mobId )
	{
		enemyGroupData = DataManager.Instance.EnemyGroupList[mobId];
		// 		// Create object pool in first battle
		// 		if ( isInitialized )
		// 			return;
		// 
		// 		// Create object pool for battle area
		// 		LgsObjectPoolManager.Instance.CreateObjectPool( enemyPrefab.name , enemyPrefab , 10 );
		// 
		// 		// Set initilize flag
		// 		isInitialized = true;
	}

	// Instantiate enemy object, set status and position in battle area. 
	private void LoadEnemyData()
	{
		// Load enemy count and check validation
		int enemyCount = enemyGroupData.enemies.Length;
		if ( enemyCount > mobPositions.Length )
		{
			Debug.LogError( "Error(battle area) : Two many enemies loaded. current " + enemyCount + ", it have to be LE " + mobPositions.Length );
		}

		// Set the script in enemy instance
		enemyInstanceList = new GameObject[enemyCount];
		for ( int i = 0 ; i < enemyCount ; ++i )
		{
			// Instance enemy and set the status data
			string mobTypeName = ( enemyGroupData.enemies[i].mobType ).ToString();
			enemyInstanceList[i] = LgsObjectPoolManager.Instance.ObjectPools[mobTypeName].PullObject();
			enemyInstanceList[i].GetComponent<Mob>().InitMob( enemyGroupData.enemies[i].characterData );
			mobHpBar[i].GetComponent<HPBar>().InitHpBar( EnemyInstanceList[i] );
			//enemyInstanceList[i].AddComponent( "Enemy" );
			//enemyInstanceList[i].GetComponent<Enemy>().EnemyStat = enemyGroupData.enemies[i].characterData;


			// Put enemy own position
			//enemyInstanceList[i].transform.parent = mobPositions[i].transform;
			enemyInstanceList[i].transform.position = mobPositions[i].transform.position;
			enemyInstanceList[i].transform.rotation = mobPositions[i].transform.rotation;
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
		for ( int i = 0 ; i < heroCount ; ++i )
		{
			heroStatus[i].GetComponent<Hero>().HeroStat = DataManager.Instance.HeroList[i];
		}

		// Move camera on predetermined position in battle area
		EnvironmentManager.Instance.PutCamera( playerPosition , CameraMode.FIRST_PERSON );
	}

}
