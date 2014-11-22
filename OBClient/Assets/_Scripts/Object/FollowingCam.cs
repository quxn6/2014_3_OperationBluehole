using UnityEngine;
using System.Collections;

public class FollowingCam : MonoBehaviour 
{
	private GameObject followingObject = null;
	private Vector3 followingPosition;
	private Vector3 followingRotation;
	private bool initFlag = false;

	public void SetFollowingTarget( GameObject followingObject, Vector3 followingPosition, Vector3 followingRotation)
	{
		this.followingObject = followingObject;
		this.followingPosition = followingPosition;
		this.followingRotation = followingRotation;
		initFlag = true;
	}

	void Update()
	{
		if ( initFlag )
		{
			transform.position = followingObject.transform.position + followingPosition;
			transform.eulerAngles = /*followingObject.transform.eulerAngles +*/ followingRotation;
		}
	}
}
