using UnityEngine;
using System.Collections;

public class HYObjectPool : ScriptableObject
{
	private GameObject[] objectPool;
	private GameObject root;
	private int currentPoolSize;
	private int currentIndex;
	private int instanciatedObjectIndex;
	private string objectName;
	private bool isInitialized = false;

	public void InitPool( GameObject prefabObject , int defaultPoolSize )
	{
		if ( isInitialized )
		{
			Debug.LogError( "It's already initialized" );
			return;
		}
		currentIndex = 0;
		instanciatedObjectIndex = 0;
		currentPoolSize = defaultPoolSize;
		objectName = prefabObject.name;
		objectPool = new GameObject[currentPoolSize];
		root = new GameObject( objectName + "_Root" );

		InstanceObjects( objectPool, prefabObject , currentPoolSize );

		isInitialized = true;
	}

	private void InstanceObjects( GameObject[] objectPool, GameObject prefabObject , int poolsize )
	{
		//for ( int i = 0 ; i < currentPoolSize ; ++i )
		while ( instanciatedObjectIndex  < poolsize )
		{
			objectPool[instanciatedObjectIndex] = Instantiate( prefabObject ) as GameObject;
			objectPool[instanciatedObjectIndex].name = objectName + "_" + instanciatedObjectIndex;
			objectPool[instanciatedObjectIndex].SetActive( false );
			objectPool[instanciatedObjectIndex].transform.parent = root.transform;
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
		InstanceObjects( newPool, objectPool[0] , newPoolSize );

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

		string[] nameParse = System.Text.RegularExpressions.Regex.Split( instanceObject.name , objectName + "_" );
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
		isInitialized = false;
	}

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
