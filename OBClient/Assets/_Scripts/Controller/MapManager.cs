using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// raw data for map
public struct Dungeon
{
	public char[,] mapArray2D;
	public int size;

	public Dungeon( char[,] dungeonMap , int size )
	{
		//this.dungeonMap = dungeonMap.ToCharArray();
		this.mapArray2D = dungeonMap;
		this.size = size;
	}
}

// dungeon creator, do object placement
// i : vertical , j : horizontal
public class MapManager : MonoBehaviour
{
	static private bool isInitialized = false;

	public GameObject wallPrefab;
	public GameObject playerPrefab;
	public GameObject emptySpacePrefab;
	public GameObject itemBoxPrefab;
	public GameObject floorPrefab;
	public GameObject[] mobPrefab;	

	//private char[] currentMapInfo;
	private Dungeon instanceDungeon;
	public Dungeon InstanceDungeon
	{
		get { return instanceDungeon; }
		set { instanceDungeon = value; }
	}

	private GameObject playerParty;
	public UnityEngine.GameObject PlayerParty
	{
		get { return playerParty; }
	}

	public Dictionary<int , GameObject> MobList { get; private set; }

	private List<GameObject> itemList;
	public List<GameObject> ItemList
	{
		get { return itemList; }
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
		MobList = new Dictionary<int , GameObject>();
		itemList = new List<GameObject>();
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

		for ( int i = 0 ; i < mobPrefab.Length ; ++i )
		{
			string typeName = ( (MobType)i ).ToString();
			LgsObjectPoolManager.Instance.CreateObjectPool( typeName , mobPrefab[i] , 32 );
		}


		// set initilize flag
		isInitialized = true;
	}

	public void PutMapObjects( Dungeon dungeonMap )
	{
		// Load map data from server
		instanceDungeon = dungeonMap;
		instanceDungeon.mapArray2D = (char[,])dungeonMap.mapArray2D.Clone();

		// Put Game object on compatible coordinate of the map 
		GameObject floor = LgsObjectPoolManager.Instance.ObjectPools[floorPrefab.name].PullObject();
		floor.transform.position = new Vector3( instanceDungeon.size / 2.0f , 0.0f , instanceDungeon.size / 2.0f );
		floor.transform.rotation = Quaternion.Euler( 90.0f , 0.0f , 0.0f );
		floor.transform.localScale = new Vector3( (float)instanceDungeon.size , (float)instanceDungeon.size , 1.0f );

		for ( int i = 0 ; i < instanceDungeon.size; ++i )
		{
			for ( int j = 0 ; j < instanceDungeon.size ; ++j )
			{
				switch ( instanceDungeon.mapArray2D[i,j] )
				{
					case '#':
						InstantiateObject( emptySpacePrefab.name , i, j );
						break;
					case 'X':
						InstantiateObject( wallPrefab.name , i, j );
						break;
					case 'P':
						playerParty = InstantiateObject( playerPrefab.name , i, j );
						EnvironmentManager.Instance.PutCamera( playerParty , CameraMode.THIRD_PERSON );
						break;
					case 'I':
						InstanceItem( i,j );
						break;
					case 'M':
						InstanceMob( i,j );
						break;
					case 'O':
						// exit object
						break;
					case 'T': // Mob on the item
						InstantiateObject( itemBoxPrefab.name , i, j );
						InstanceMob( i,j );
						break;
				}
			}
		}
	}

	private void InstanceItem( int i, int j )
	{
		int itemId = itemList.Count;
		itemList.Add( InstantiateObject( itemBoxPrefab.name , i, j ) );
	}

	private int mobIterator = 0;
	private void InstanceMob( int i, int j )
	{
		// Mob Model in the dungeon will be first mob in the Mob Group;
		int mobId = MobList.Count;
		//MobType newMobType = DataManager.Instance.EnemyGroupList[mobId].enemies[0].mobType;		
		OperationBluehole.Content.MobType newMobType = ( (OperationBluehole.Content.Mob)DataManager.Instance.MobPartyList[mobIterator++].characters[0] ).mobType;
		GameObject mobInstance = InstantiateObject( newMobType.ToString() , i, j);
		//mobInstance.GetComponent<Mob>().MobId = mobId;
		MobList.Add( i * instanceDungeon.size + j , mobInstance);
		//MobList.Add( mobInstance );
	}

	public void ClearAllMapObjects()
	{
		LgsObjectPoolManager.Instance.ResetAllObjectPools();
		MobList.Clear();
	}

	private GameObject InstantiateObject( string prefabObjectName , int i, int j )
	{
		LgsObjectPool pool;
		if ( !LgsObjectPoolManager.Instance.ObjectPools.TryGetValue( prefabObjectName , out pool ) )
		{
			Debug.LogError( "No Object in pool name of " + prefabObjectName );
		}

		GameObject instanceObject = LgsObjectPoolManager.Instance.ObjectPools[prefabObjectName].PullObject();
		//instanceObject.transform.Translate( (float)( x % instanceDungeon.size ) , 0.0f , (float)( x / instanceDungeon.size ) );		
		instanceObject.transform.Translate( (float)j , 0.0f , (float)i);		
		return instanceObject;
	}

}
