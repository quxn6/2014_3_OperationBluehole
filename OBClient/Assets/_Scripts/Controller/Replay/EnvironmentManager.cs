using UnityEngine;
using System.Collections;

public enum CameraMode
{
	FIRST_PERSON,
	THIRD_PERSON,
}

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

	public void PutCamera(GameObject followingObject, CameraMode cameraMode)
	{		
		switch ( cameraMode )
		{
			case CameraMode.THIRD_PERSON:
				//mainCamera.transform.position = followingObject.transform.position + GameConfig.CAMERA_THIRD_PERSON_POSITION;
				mainCamera.GetComponent<FollowingCam>().SetFollowingTarget(
					followingObject ,
					GameConfig.CAMERA_THIRD_PERSON_POSITION ,
					GameConfig.CAMERA_THIRD_PERSON_ANGLE
					);
				break;
			case CameraMode.FIRST_PERSON:
				mainCamera.GetComponent<FollowingCam>().SetFollowingTarget(
					followingObject ,
					GameConfig.CAMERA_FIRST_PERSON_POSITION ,
					GameConfig.CAMERA_FIRST_PERSON_ANGLE
					);
				break;
		}

// 		mainCamera.transform.parent = followingObject.transform;
// 		switch (cameraMode)
// 		{
// 			case CameraMode.THIRD_PERSON:
// 				//mainCamera.transform.position = followingObject.transform.position + GameConfig.CAMERA_THIRD_PERSON_POSITION;
// 				mainCamera.transform.localPosition = GameConfig.CAMERA_THIRD_PERSON_POSITION;
// 				mainCamera.transform.localEulerAngles = GameConfig.CAMERA_THIRD_PERSON_ANGLE ;				
// 				break;
// 			case CameraMode.FIRST_PERSON:
// 				mainCamera.transform.localPosition = GameConfig.CAMERA_FIRST_PERSON_POSITION;
// 				mainCamera.transform.localEulerAngles = GameConfig.CAMERA_FIRST_PERSON_ANGLE ;				
// 				break;
// 		}
	}
}
