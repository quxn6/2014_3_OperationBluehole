﻿using UnityEngine;
using System.Collections;

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

// dungeon creator, do object placement
public class MapManager : MonoBehaviour
{
	public GameObject wallPrefab;
	public GameObject playerPrefab;
	public GameObject emptySpacePrefab;
	public GameObject itemBoxPrefab;
	public GameObject floorPrefab;
	public GameObject mobPrefab;

	private Dungeon instanceDungeon;
	private GameObject playerInstance;

	static private MapManager instance;
	static public MapManager Instance
	{
		get { return instance; }		
	}

	void Awake()
	{
		if ( null != instance )
		{
			Debug.LogError( "MapManager already exist" );
			return;
		}

		instance = this;
	}

	public void InitMapObjects()
	{		
		LgsObjectPoolManager.Instance.CreateObjectPool( wallPrefab.name , wallPrefab , 512 );
		LgsObjectPoolManager.Instance.CreateObjectPool( playerPrefab.name , playerPrefab , 1 );
		LgsObjectPoolManager.Instance.CreateObjectPool( emptySpacePrefab.name , emptySpacePrefab , 2048 );
		LgsObjectPoolManager.Instance.CreateObjectPool( itemBoxPrefab.name , itemBoxPrefab , 128 );
		LgsObjectPoolManager.Instance.CreateObjectPool( floorPrefab.name , floorPrefab , 1 );
		LgsObjectPoolManager.Instance.CreateObjectPool( mobPrefab.name, mobPrefab, 128 );
	}

	public void PutMapObjects(Dungeon dungeonMap)
	{
		// Load map data from server
		instanceDungeon = dungeonMap;

		// Put Game object on compatible coordinate of the map 
		GameObject floor = LgsObjectPoolManager.Instance.ObjectPools[floorPrefab.name].PullObject();
		floor.transform.position =  new Vector3( instanceDungeon.size / 2.0f , 0.0f , instanceDungeon.size / 2.0f ) ;
		floor.transform.rotation = Quaternion.Euler( 90.0f , 0.0f , 0.0f );		
		floor.transform.localScale = new Vector3( (float)instanceDungeon.size , (float)instanceDungeon.size , 1.0f );

		for ( int i = 0 ; i < instanceDungeon.dungeonMap.Length ; ++i )
		{
			switch ( instanceDungeon.dungeonMap[i] )
			{
				case 'O':
					InstancingObject( emptySpacePrefab , i );
					break;
				case 'X':
					InstancingObject( wallPrefab , i );
					break;
				case 'P':
					playerInstance = InstancingObject( playerPrefab , i );
					EnvironmentManager.Instance.PutCamera( playerInstance );
					break;
				case 'I':
					InstancingObject( itemBoxPrefab , i );
					break;
				case 'M':
					InstancingObject( mobPrefab , i );
					break;
			}
		}
	}

	private GameObject InstancingObject( GameObject prefabObject, int instanceOrder )
	{
		GameObject instanceObject = LgsObjectPoolManager.Instance.ObjectPools[prefabObject.name].PullObject();
		instanceObject.transform.Translate( (float)( instanceOrder % instanceDungeon.size ) , 0.0f , (float)( instanceOrder / instanceDungeon.size ) );		//instanceObject.transform.position = new Vector3( (float)( instanceOrder % instanceDungeon.size ) , 1.0f , (float)( instanceOrder / instanceDungeon.size ) );
		return instanceObject;
	}

}
