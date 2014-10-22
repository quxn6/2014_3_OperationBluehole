using UnityEngine;
using System.Collections;

public class EnvironmentManager : MonoBehaviour {
	public GameObject mainCamera;

	static private EnvironmentManager instance;
	static public EnvironmentManager Instance
	{
		get { return instance; }		
	}
	
	void Awake()
	{
		instance = this;
	}

	public void PutCamera(GameObject followingObject)
	{
		mainCamera.transform.position = followingObject.transform.position + new Vector3( 0.0f ,7.0f , -4.0f );
		mainCamera.transform.parent = followingObject.transform;
	}

}
