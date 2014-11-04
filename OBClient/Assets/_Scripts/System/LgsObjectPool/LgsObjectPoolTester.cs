using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LgsObjectPoolTester : MonoBehaviour
{
	public GameObject prefabObj;
	public GameObject prefabObj2;

	//private LgsObjectPool testPool;
	private Stack<GameObject> tempStack;

	void Awake()
	{
		//testPool = ScriptableObject.CreateInstance<LgsObjectPool>();
		tempStack = new Stack<GameObject>();
	}

	public void InitPool()
	{
		LgsObjectPoolManager.Instance.CreateObjectPool( "cube" , prefabObj , 20 );
		LgsObjectPoolManager.Instance.CreateObjectPool( "fafa" , prefabObj2 );
		//testPool.InitPool( prefabObj , 10, TODO );
	}

	public void ClearPool()
	{
		//testPool.ClearPool();
		LgsObjectPoolManager.Instance.EmptyAllObjectPools();
	}

	public void PullObject()
	{
		GameObject instance = LgsObjectPoolManager.Instance.ObjectPools["cube"].PullObject();
		instance.transform.position = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(0.0f, 10.0f) );
		tempStack.Push( instance );
	}

	public void PushObject()
	{
		if ( tempStack.Count == 0)
		{
			return; 
		}
		LgsObjectPoolManager.Instance.ObjectPools["cube"].PushObject( tempStack.Pop() );		
	}

	public void ResetPool()
	{
		LgsObjectPoolManager.Instance.ResetAllObjectPools();
	}

	void OnGUI()
	{
		if ( GUI.Button( new Rect( 10 , 10 , 100 , 50 ) , "Init Pool" ) )
		{
			InitPool();
		}

		if ( GUI.Button( new Rect( 10 , 70 , 100 , 50 ) , "pull Object" ) )
		{
			PullObject();
		}

		if ( GUI.Button( new Rect( 10 , 130 , 100 , 50 ) , "push Object" ) )
		{
			PushObject();
		}

		if ( GUI.Button( new Rect( 10 , 190 , 100 , 50 ) , "Clear Pool" ) )
		{
			ClearPool();
		}

		if ( GUI.Button( new Rect( 10 , 250 , 100 , 50 ) , "Reset Pool" ) )
		{
			ResetPool();
		}

		if ( GUI.Button( new Rect( 120 , 10 , 100 , 50 ) , "Load Level" ) )
		{
			Application.LoadLevel( "LgsObjectPoolTest" );
		}


	}
}
