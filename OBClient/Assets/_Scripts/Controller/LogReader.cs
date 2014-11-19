using UnityEngine;
using System.Collections;

public class LogReader : MonoBehaviour 
{
	static private LogReader instance;
	static public LogReader Instance
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

	//// warning!!! need to Merge with DataManager
	public void InitLog()
	{


	}
}
