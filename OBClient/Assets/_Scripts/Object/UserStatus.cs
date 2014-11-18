using UnityEngine;
using System.Collections;

public class UserStatus : MonoBehaviour
{
	void Update()
	{
		Vector3 charactorToCamera = Camera.main.transform.position - gameObject.transform.position;
		transform.rotation = Quaternion.Euler( new Vector3( 0.0f , -160.0f + Mathf.Rad2Deg * Mathf.Atan( charactorToCamera.x / charactorToCamera.z ) , 0.0f ) );
	}
}
