using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LgsObjectPoolManager : MonoBehaviour {
	public bool isDontDestroyOnLoad = true;

	static private LgsObjectPoolManager instance;
	static public LgsObjectPoolManager Instance
	{
		get { return instance; }		
	}

	private const int DEFAULT_POOLSIZE = 32;
	private Dictionary<string, LgsObjectPool> objectPools;
	public Dictionary<string , LgsObjectPool> ObjectPools
	{
		get { return objectPools; }
	}
	// Get object pool
// 	public LgsObjectPool GetObjectPool(string poolName)
// 	{
// 		LgsObjectPool pool;
// 		if ( !objectPools.TryGetValue( poolName , out pool ) )
// 		{
// 			Debug.LogError("Find Error : There is no pool that has name of "+poolName);
// 		}
// 		return pool;
// 	}
	
	void Awake()
	{
		if ( null != instance )
		{
			Debug.LogWarning( "Warning : There is another ObjectPoolManager, It can be exist only one manager at the same time" );
			Destroy( gameObject );
			return;
		}

		instance = this;
		objectPools = new Dictionary<string, LgsObjectPool>();
		if (isDontDestroyOnLoad) 
			DontDestroyOnLoad( gameObject );
	}

	// Create object pool
	public bool CreateObjectPool(string poolName, GameObject prefab, int poolSize = DEFAULT_POOLSIZE)
	{
		if ( objectPools.ContainsKey(poolName) )
		{
			Debug.LogWarning( "Create Warning : There is already exist that has same name you want to create" );
			return false;
		}
		LgsObjectPool objectPoolInstance = ScriptableObject.CreateInstance<LgsObjectPool>();
		objectPoolInstance.InitPool(prefab, poolSize, gameObject);
		objectPools.Add( poolName , objectPoolInstance );
		return true;
	}

	// remove specific object pool
	public bool RemoveObjectPool(string poolName)
	{
		if ( !objectPools.ContainsKey( poolName ) )
		{
			Debug.LogError( "Remove Error : There is no pool that has name of " + poolName );
			return false;
		}
			
		objectPools[poolName].ClearPool();
		objectPools.Remove( poolName );
		return true;
	}

	// Deactivate all objects of each pool
	public void ResetAllObjectPools()
	{
		foreach(LgsObjectPool pool in objectPools.Values)
		{
			pool.ResetPool();
		}
	}

	// Remove all object pools
	public void EmptyAllObjectPools()
	{
		if ( 0 == objectPools.Count )
		{
			Debug.Log( "It's empty already" );
			return;
		}

		foreach ( LgsObjectPool pool in objectPools.Values )
		{
			pool.ClearPool();
		}
		objectPools.Clear();
	}	

	public bool ContainsPool(string name)
	{
		return objectPools.ContainsKey( name );
	}
}
