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

	public GameObject[] wallPrefab;	
	public GameObject[] thinWallPrefab;
	public GameObject[] fencePrefab;
	public GameObject[] playerPrefab;
	public GameObject[] itemBoxPrefab;
	public GameObject[] floorPrefab;

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

	public Dictionary<int , GameObject> MobDictionary { get; private set; }
	public Dictionary<int , GameObject> ItemDictionary { get; private set; }

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
		MobDictionary = new Dictionary<int , GameObject>();
		ItemDictionary = new Dictionary<int , GameObject>();
	}

	// initialize objectpool only one time
	public void InitMapObjects()
	{
		// check it's already initialized in another dungeon
		// User replay dungeon at least one time before, this flag was set up
		if ( isInitialized )
			return;

		// create dungeon object pool in object pool Manager
		CreatePool( wallPrefab , 512 );
		CreatePool( thinWallPrefab , 128 );
		CreatePool( fencePrefab , 128 );
		CreatePool( playerPrefab, 1);
		CreatePool( itemBoxPrefab, 128 );
		CreatePool( floorPrefab, 128 );
		for ( int i = 0 ; i < mobPrefab.Length ; ++i )
		{
			string typeName = ( (MobType)i ).ToString();
			LgsObjectPoolManager.Instance.CreateObjectPool( typeName , mobPrefab[i] , 32 );
		}


		// set initilize flag
		isInitialized = true;
	}

	private void CreatePool(GameObject[] prefabs, int size)
	{
		for ( int i = 0 ; i < prefabs.Length ; ++i)
		{
			LgsObjectPoolManager.Instance.CreateObjectPool( prefabs[i].name , prefabs[i] , size );
		}
	}

	enum MapObjectType
	{
		None = 0,
		ThinWall = 1,
		ThinFence = 2,
		ThinEmpty = 4,
		ThinMask = ThinWall + ThinFence + ThinEmpty,
		HasFloor = 8,
	}

	enum ObjectDirection
	{
		Vertical = 0,
		Horizontal = 1,
	}

	public void PutMapObjects( Dungeon dungeonMap )
	{
		// Load map data from server
		instanceDungeon = dungeonMap;
		instanceDungeon.mapArray2D = (char[,])dungeonMap.mapArray2D.Clone();

		// Put Game object on compatible coordinate of the map 
// 		GameObject floor = LgsObjectPoolManager.Instance.ObjectPools[floorPrefab[0].name].PullObject();
// 		floor.transform.position = new Vector3( instanceDungeon.size / 2.0f , 0.0f , instanceDungeon.size / 2.0f );
// 		floor.transform.rotation = Quaternion.Euler( 90.0f , 0.0f , 0.0f );
// 		floor.transform.localScale = new Vector3( (float)instanceDungeon.size , (float)instanceDungeon.size , 1.0f );

		MapObjectType[,] isPlaced = new MapObjectType[dungeonMap.size , dungeonMap.size];
		for ( int r = 0 ; r < dungeonMap.size ; ++r )
		{
			for ( int c = 0 ; c < dungeonMap.size ; ++c)
			{
				isPlaced[r,c] = MapObjectType.None;
			}
		}

		for ( int i = 0 ; i < instanceDungeon.size ; ++i )
		{
			for ( int j = 0 ; j < instanceDungeon.size ; ++j )
			{
				// check flag
				if ( (isPlaced[i,j] & MapObjectType.HasFloor) == 0)
				{
					if ( Random.Range( 0 , 10 ) == 0 )
					{
						InstantiateObject( floorPrefab[Random.Range( 1 , floorPrefab.Length )].name , i , j );
					}
					else
					{
						InstantiateObject( floorPrefab[0].name , i , j );
					}

					for ( int k = i ; k < i + GameConfig.FLOOR_BLOCK_SIZE ; ++k )
					{
						for ( int l = j ; l < j + GameConfig.FLOOR_BLOCK_SIZE ; ++l )
						{
							isPlaced[k , l] |= MapObjectType.HasFloor;
						}
					}
				}
				
				

				if ( (isPlaced[i,j] & MapObjectType.ThinMask) != 0 )
				{
					continue;
				}

				switch ( instanceDungeon.mapArray2D[i , j] )
				{
					case 'X':
						if ( !CanBePlaced(instanceDungeon.size, i, j))
						{
							InstantiateObject( wallPrefab[Random.Range( 0 , wallPrefab.Length )].name , i , j );
							continue;
						}
						
						// Check thin wall can be placed
						// Warning!!.. this is not a code, this is just set of sXXX..... I swear I never wright like this again. T_T
						if ( instanceDungeon.mapArray2D[i , j + 1] == 'X' && instanceDungeon.mapArray2D[i , j - 1] != 'X' && instanceDungeon.mapArray2D[i , j - 1] != '#' && instanceDungeon.mapArray2D[i , j + 2] != 'X' && instanceDungeon.mapArray2D[i , j + 2] != '#' )
						{
							if ( (isPlaced[i - 1 , j] & MapObjectType.ThinFence) != 0)
							{
								GameObject thinObject = InstantiateObject( fencePrefab[Random.Range( 0 , fencePrefab.Length )].name , i , j );
								thinObject.transform.Rotate( new Vector3( 0.0f , 90.0f , 0.0f ) );
								isPlaced[i , j] |= MapObjectType.ThinFence;
								isPlaced[i , j + 1] |= MapObjectType.ThinEmpty;
							}
								
							if ( (isPlaced[i - 1, j ] & MapObjectType.ThinWall ) != 0)
							{
								GameObject thinObject = InstantiateObject( thinWallPrefab[Random.Range( 0 , thinWallPrefab.Length )].name , i , j );
								thinObject.transform.Rotate( new Vector3( 0.0f , 90.0f , 0.0f ) );
								isPlaced[i , j] |= MapObjectType.ThinWall;
								isPlaced[i , j + 1] |= MapObjectType.ThinEmpty;
							}
								
							if ( (isPlaced[i -1, j ] & MapObjectType.ThinMask) == 0 )
							{
								if (Random.Range(0,2) == 0)
								{
									GameObject thinObject = InstantiateObject( fencePrefab[Random.Range( 0 , fencePrefab.Length )].name , i , j );
									thinObject.transform.Rotate( new Vector3( 0.0f , 90.0f , 0.0f ) );
									isPlaced[i , j] |= MapObjectType.ThinFence;
									isPlaced[i , j + 1] |= MapObjectType.ThinEmpty;
								}
								else
								{
									GameObject thinObject = InstantiateObject( thinWallPrefab[Random.Range( 0 , thinWallPrefab.Length )].name , i , j );
									thinObject.transform.Rotate( new Vector3( 0.0f , 90.0f , 0.0f ) );
									isPlaced[i , j] |= MapObjectType.ThinWall;
									isPlaced[i , j + 1] |= MapObjectType.ThinEmpty;
								}
							}
						}
						else if ( instanceDungeon.mapArray2D[i , j - 1] != '#' && instanceDungeon.mapArray2D[i , j - 1] != 'X' && instanceDungeon.mapArray2D[i , j + 1] == '#' && instanceDungeon.mapArray2D[i , j + 1] == 'X' )
						{
							if ( (isPlaced[i - 1 , j] & MapObjectType.ThinFence) != 0)
							{
								GameObject thinObject = InstantiateObject( fencePrefab[Random.Range( 0 , fencePrefab.Length )].name , i , j );
								thinObject.transform.Rotate( new Vector3( 0.0f , 90.0f , 0.0f ) );
								isPlaced[i , j] |= MapObjectType.ThinFence;
							}

							if ( (isPlaced[i - 1 , j] & MapObjectType.ThinWall) != 0)
							{
								GameObject thinObject = InstantiateObject( thinWallPrefab[Random.Range( 0 , thinWallPrefab.Length )].name , i , j );
								thinObject.transform.Rotate( new Vector3( 0.0f , 90.0f , 0.0f ) );
								isPlaced[i , j] |= MapObjectType.ThinWall;
							}

							if ( (isPlaced[i - 1 , j] & MapObjectType.ThinMask) == 0 )
							{
								if ( Random.Range( 0 , 2 ) == 0 )
								{
									GameObject thinObject = InstantiateObject( fencePrefab[Random.Range( 0 , fencePrefab.Length )].name , i , j );
									thinObject.transform.Rotate( new Vector3( 0.0f , 90.0f , 0.0f ) );
									isPlaced[i , j] |= MapObjectType.ThinFence;
								}
								else
								{
									GameObject thinObject = InstantiateObject( thinWallPrefab[Random.Range( 0 , thinWallPrefab.Length )].name , i , j );
									thinObject.transform.Rotate( new Vector3( 0.0f , 90.0f , 0.0f ) );
									isPlaced[i , j] |= MapObjectType.ThinWall;
								}
							}
						}
						else if ( instanceDungeon.mapArray2D[i + 1 , j] == 'X' && instanceDungeon.mapArray2D[i - 1 , j] != 'X' && instanceDungeon.mapArray2D[i - 1 , j] != '#' && instanceDungeon.mapArray2D[i + 2 , j] != 'X' && instanceDungeon.mapArray2D[i + 2 , j] != '#' )
						{
							if ( (isPlaced[i , j - 1] & MapObjectType.ThinFence) != 0 )
							{
								InstantiateObject( fencePrefab[Random.Range( 0 , fencePrefab.Length )].name , i , j );
								isPlaced[i , j] |= MapObjectType.ThinFence;
								isPlaced[i + 1 , j] |= MapObjectType.ThinEmpty;
							}

							if ( (isPlaced[i , j - 1] & MapObjectType.ThinWall) != 0 )
							{
								InstantiateObject( thinWallPrefab[Random.Range( 0 , thinWallPrefab.Length )].name , i , j );
								isPlaced[i , j] |= MapObjectType.ThinWall;
								isPlaced[i + 1 , j] |= MapObjectType.ThinEmpty;
							}

							if ( (isPlaced[i , j - 1] & MapObjectType.ThinMask) == 0 )
							{
								if ( Random.Range( 0 , 2 ) == 0 )
								{
									InstantiateObject( fencePrefab[Random.Range( 0 , fencePrefab.Length )].name , i , j );
									isPlaced[i , j] |= MapObjectType.ThinFence;
								}
								else
								{
									InstantiateObject( thinWallPrefab[Random.Range( 0 , thinWallPrefab.Length )].name , i , j );
									isPlaced[i , j] |= MapObjectType.ThinWall;
								}
							}
						}
						else if ( instanceDungeon.mapArray2D[i - 1 , j] != '#' && instanceDungeon.mapArray2D[i - 1 , j] != 'X' && instanceDungeon.mapArray2D[i + 1 , j] != 'X' && instanceDungeon.mapArray2D[i + 1 , j] != '#' )
						{
							if ( (isPlaced[i , j - 1] & MapObjectType.ThinFence) != 0 )
							{
								InstantiateObject( fencePrefab[Random.Range( 0 , fencePrefab.Length )].name , i , j );
								isPlaced[i , j] |= MapObjectType.ThinFence;
							}

							if ( (isPlaced[i , j - 1] & MapObjectType.ThinWall) != 0 )
							{
								InstantiateObject( thinWallPrefab[Random.Range( 0 , thinWallPrefab.Length )].name , i , j );
								isPlaced[i , j] |= MapObjectType.ThinWall;
							}

							if ( (isPlaced[i , j - 1] & MapObjectType.ThinMask) == 0 )
							{
								if ( Random.Range( 0 , 2 ) == 0 )
								{
									InstantiateObject( fencePrefab[Random.Range( 0 , fencePrefab.Length )].name , i , j );
									isPlaced[i , j] |= MapObjectType.ThinFence;
								}
								else
								{
									InstantiateObject( thinWallPrefab[Random.Range( 0 , thinWallPrefab.Length )].name , i , j );
									isPlaced[i , j] |= MapObjectType.ThinWall;
								}
							}
						}
						else
						{
							InstantiateObject( wallPrefab[0].name , i , j );	
						}
						

						break;
					case '#':
						InstantiateObject( wallPrefab[0].name , i , j );
						break;
					case 'P':
						playerParty = InstantiateObject( playerPrefab[0].name , i , j );
						EnvironmentManager.Instance.PutCamera( playerParty , CameraMode.THIRD_PERSON );
						break;
					case 'I':
						InstanceItem( i , j );
						break;
					case 'M':
						InstanceMob( i , j );
						break;
					case 'O':
						// exit object
						break;
					case 'T': // Mob on the item
						InstanceItem( i , j );
						InstanceMob( i , j );
						break;
				}
			}
		}
	}

	private void CreateThinBlock(MapObjectType isPlaced, ObjectDirection objectDirection, int i, int j)
	{

	}

	private bool CanBePlaced( int size, int col, int row )
	{
		if ( col < 1 || row < 1 || col > size - 3 || row > size - 3 )
			return false;
		else
			return true;
	}

	private void PlaceItemVertical( int i, int j)
	{

	}

	private void InstanceItem( int i , int j )
	{
		int itemId = ItemDictionary.Count;
		ItemDictionary.Add( i * instanceDungeon.size + j , InstantiateObject( itemBoxPrefab[0].name , i , j ) );
	}

	private int mobIterator = 0;
	private void InstanceMob( int i , int j )
	{
		// Mob Model in the dungeon will be first mob in the Mob Group;
		int mobId = MobDictionary.Count;
		//MobType newMobType = DataManager.Instance.EnemyGroupList[mobId].enemies[0].mobType;		
		OperationBluehole.Content.MobType newMobType = ( (OperationBluehole.Content.Mob)DataManager.Instance.MobPartyList[mobIterator++].characters[0] ).mobType;
		GameObject mobInstance = InstantiateObject( newMobType.ToString() , i , j );
		MobDictionary.Add( i * instanceDungeon.size + j , mobInstance );
		//MobList.Add( mobInstance );
	}

	public void ClearAllMapObjects()
	{
		LgsObjectPoolManager.Instance.ResetAllObjectPools();
		MobDictionary.Clear();
	}

	private GameObject InstantiateObject( string prefabObjectName , int i , int j )
	{
		LgsObjectPool pool;
		if ( !LgsObjectPoolManager.Instance.ObjectPools.TryGetValue( prefabObjectName , out pool ) )
		{
			Debug.LogError( "No Object in pool name of " + prefabObjectName );
		}

		GameObject instanceObject = LgsObjectPoolManager.Instance.ObjectPools[prefabObjectName].PullObject();
		//instanceObject.transform.Translate( (float)( x % instanceDungeon.size ) , 0.0f , (float)( x / instanceDungeon.size ) );		
		instanceObject.transform.Translate( (float)j , 0.0f , (float)i );
		return instanceObject;
	}

}
