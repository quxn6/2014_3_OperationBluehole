using UnityEngine;
using System.Collections;

public class ItemManager : MonoBehaviour
{
	static private ItemManager instance = null;
	static public ItemManager Instance
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
}
