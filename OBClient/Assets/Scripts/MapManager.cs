using UnityEngine;
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

	public GameObject mapRoot;
	public GameObject wallRoot;
	public GameObject emptySpaceRoot;
	public GameObject itemBoxRoot;
	public GameObject mobRoot;

	private Dungeon instanceDungeon;
	private GameObject playerInstance;
	
	public void SetMapObjects(Dungeon dungeonMap)
	{
		// Load map data from server
		instanceDungeon = dungeonMap;

		// Put Game object on compatible coordinate of the map 
		GameObject floor = Instantiate( floorPrefab , new Vector3( instanceDungeon.size / 2.0f , 0.0f , instanceDungeon.size / 2.0f ) , Quaternion.Euler( 90.0f , 0.0f , 0.0f ) ) as GameObject;
		floor.transform.localScale = new Vector3( (float)instanceDungeon.size , (float)instanceDungeon.size , 1.0f );

		for ( int i = 0 ; i < instanceDungeon.dungeonMap.Length ; ++i )
		{
			switch ( instanceDungeon.dungeonMap[i] )
			{
				case 'O':
					InstancingObject( emptySpacePrefab , emptySpaceRoot , i );
					break;
				case 'X':
					InstancingObject( wallPrefab , wallRoot , i );
					break;
				case 'P':
					playerInstance = InstancingObject( playerPrefab , mapRoot , i );
					EnvironmentManager.Instance.PutCamera( playerInstance );
					break;
				case 'I':
					InstancingObject( itemBoxPrefab , itemBoxRoot , i );
					break;
				case 'M':
					InstancingObject( mobPrefab , mobRoot , i );
					break;
			}
		}
	}

	private GameObject InstancingObject( GameObject prefabObject, GameObject parentObject, int instanceOrder )
	{
		GameObject instanceObject = Instantiate( prefabObject ) as GameObject;
		//instanceObject.transform.position = new Vector3( (float)( instanceOrder % instanceDungeon.size ) , 1.0f , (float)( instanceOrder / instanceDungeon.size ) );
		instanceObject.transform.Translate( (float)( instanceOrder % instanceDungeon.size ) , 0.0f , (float)( instanceOrder / instanceDungeon.size ) );
		instanceObject.name = prefabObject.name + instanceOrder;
		instanceObject.transform.parent = parentObject.transform;
		return instanceObject;
	}

}
