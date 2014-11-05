using UnityEngine;
using System.Collections;

public class LgsObjectPool : ScriptableObject
{
	private GameObject objectPoolRoot;
	private GameObject[] objectPool;
	private GameObject objectList;
	private GameObject originalObject;
	private int currentPoolSize = 0;
	private int currentIndex = 0;
	private int instanciatedObjectIndex = 0;
	private bool isInitialized = false;

	public void InitPool( GameObject prefabObject , int defaultPoolSize, GameObject root )
	{		
		if ( isInitialized )
		{
			Debug.LogError( "It's already initialized" );
			return;
		}

		// Manage ObjectPools
		objectPoolRoot = root;	
				

		// init current pool variables
		currentIndex = 0;
		instanciatedObjectIndex = 0;
		currentPoolSize = defaultPoolSize;
		originalObject = prefabObject;

		// Create empty object for hierarchy
		objectList = new GameObject( originalObject.name + "_list" );
		objectList.transform.parent = objectPoolRoot.transform;

		// Create Pool
		objectPool = new GameObject[currentPoolSize];
		InstanceObjects( objectPool, prefabObject , currentPoolSize );

		// Initializing Complete
		isInitialized = true;
	}	

	private void InstanceObjects( GameObject[] objectPool, GameObject prefabObject , int poolsize )
	{
		//for ( int i = 0 ; i < currentPoolSize ; ++i )
		while ( instanciatedObjectIndex  < poolsize )
		{
			objectPool[instanciatedObjectIndex] = Instantiate( prefabObject ) as GameObject;
			objectPool[instanciatedObjectIndex].name = originalObject.name + "_" + instanciatedObjectIndex;
			objectPool[instanciatedObjectIndex].SetActive( false );
			objectPool[instanciatedObjectIndex].transform.parent = objectList.transform;
			++instanciatedObjectIndex;
		}
	}

	public GameObject PullObject()
	{
		if ( !isInitialized )
		{
			Debug.LogError( "It's not initialized yet" );
			return null;
		}

		if ( currentIndex >= currentPoolSize )
		{
			currentIndex %= currentPoolSize;
		}

		int beginIndex = currentIndex;

		// find deactivated object
		while ( objectPool[currentIndex].activeSelf )
		{
			// increase index			
			++currentIndex;
			if ( currentIndex >= currentPoolSize )
			{
				currentIndex %= currentPoolSize;
			}

			// if all objects are used
			if ( currentIndex == beginIndex )
			{
				ExpandObjectPool();
			}
		}

		// return it
		objectPool[currentIndex].SetActive( true );
		return objectPool[currentIndex++];
	}
	
	private void ExpandObjectPool( int Mult = 2 )
	{
		// if pool is poor, you might see the warning message
		Debug.LogWarning( "Pool is full, you should expand defaultPoolSize, it can be cause of performance issue" );

		// create expanded pool
		int newPoolSize = currentPoolSize * Mult;
		GameObject[] newPool = new GameObject[newPoolSize];

		// copy pointer of currentPool
		for ( int i = 0 ; i < currentPoolSize ; ++i )
		{
			newPool[i] = objectPool[i];
		}

		// Create expands capacity;
		InstanceObjects( newPool, originalObject , newPoolSize );

		// replace pool
		objectPool = newPool;
		currentPoolSize = newPoolSize;
	}

	// return object
	public void PushObject( GameObject instanceObject )
	{
		if ( !isInitialized )
		{
			Debug.LogError( "It's not initialized yet" );
			return;
		}

		string[] nameParse = System.Text.RegularExpressions.Regex.Split( instanceObject.name , originalObject.name + "_" );
		if ( nameParse.Length != 2 )
		{
			Debug.Log( "This object is not a member of Object Pool" );
		}

		if ( objectPool[int.Parse( nameParse[1] )] != instanceObject )
		{
			Debug.LogError( "Object Pool Error : not matching" );
		}
		instanceObject.SetActive( false );
	}

	// remove all members
	public void ClearPool()
	{
		if ( !isInitialized )
		{
			Debug.LogError( "It's not initialized yet" );
			return;
		}

		for ( int i = 0 ; i < currentPoolSize ; ++i )
		{
			Destroy( objectPool[i] );
		}

		Destroy( objectList );
		
		// Clear Complete
		isInitialized = false;
	}

	// Deactivate all members in object pool
	public void ResetPool()
	{
		if ( !isInitialized )
		{
			Debug.LogError( "It's not initialized yet" );
			return;
		}

		foreach ( GameObject instanceObject in objectPool)		
		{
			instanceObject.SetActive( false );
		}
	}
}
