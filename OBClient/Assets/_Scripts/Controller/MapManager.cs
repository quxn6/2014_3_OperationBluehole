using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// dungeon creator, do object placement
public class MapManager : MonoBehaviour
{
	static private bool isInitialized = false;

	public GameObject wallPrefab;
	public GameObject playerPrefab;
	public GameObject emptySpacePrefab;
	public GameObject itemBoxPrefab;
	public GameObject floorPrefab;
	public GameObject mobPrefab;
	
	private Dungeon instanceDungeon;
	private GameObject playerParty;
	public UnityEngine.GameObject PlayerParty
	{
		get { return playerParty; }
	}

	private List<GameObject> mobList;
	public List<GameObject> MobList
	{
		get { return mobList; }
	}

	static private MapManager instance;
	static public MapManager Instance
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
		mobList = new List<GameObject>();
	}

	// initialize objectpool only one time
	public void InitMapObjects()
	{
		// check it's already initialized in another dungeon
		// User replay dungeon at least one time before, this flag was set up
		if ( isInitialized )
			return; 

		// create dungeon object pool in object pool Manager
		LgsObjectPoolManager.Instance.CreateObjectPool( wallPrefab.name , wallPrefab , 512 );
		LgsObjectPoolManager.Instance.CreateObjectPool( playerPrefab.name , playerPrefab , 1 );
		LgsObjectPoolManager.Instance.CreateObjectPool( emptySpacePrefab.name , emptySpacePrefab , 2048 );
		LgsObjectPoolManager.Instance.CreateObjectPool( itemBoxPrefab.name , itemBoxPrefab , 128 );
		LgsObjectPoolManager.Instance.CreateObjectPool( floorPrefab.name , floorPrefab , 1 );
		LgsObjectPoolManager.Instance.CreateObjectPool( mobPrefab.name, mobPrefab, 128 );
		
		// set initilize flag
		isInitialized = true;
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
					InstantiateObject( emptySpacePrefab , i );
					break;
				case 'X':
					InstantiateObject( wallPrefab , i );
					break;
				case 'P':
					playerParty = InstantiateObject( playerPrefab , i );
					EnvironmentManager.Instance.PutCamera( playerParty, CameraMode.THIRD_PERSON );
					break;
				case 'I':
					InstantiateObject( itemBoxPrefab , i );
					break;
				case 'M':
					GameObject mobInstance = InstantiateObject( mobPrefab , i );
					mobList.Add(mobInstance);
					break;
			}
		}
	}

	public void ClearAllMapObjects()
	{
		LgsObjectPoolManager.Instance.ResetAllObjectPools();
		mobList.Clear();
	}

	private GameObject InstantiateObject( GameObject prefabObject, int instanceOrder )
	{
		GameObject instanceObject = LgsObjectPoolManager.Instance.ObjectPools[prefabObject.name].PullObject();
		instanceObject.transform.Translate( (float)( instanceOrder % instanceDungeon.size ) , 0.0f , (float)( instanceOrder / instanceDungeon.size ) );		//instanceObject.transform.position = new Vector3( (float)( instanceOrder % instanceDungeon.size ) , 1.0f , (float)( instanceOrder / instanceDungeon.size ) );
		return instanceObject;
	}

}
