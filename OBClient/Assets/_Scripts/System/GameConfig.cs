using UnityEngine;
using System.Collections;

static class GameConfig
{
	public const float MINIMAL_UNIT_SIZE = 1.0f;
	
	public const float SPLASH_IMAGE_PLAY_TIME = 2.0f;
	
	// In Deongen Constants
	public const float CHARACTER_MOVING_TIME = 0.5f;
	public const float CHARACTER_TURNING_TIME = 0.2f;
	public const float CHARACTER_ROTATE_DEGREE = 90.0f;

	// Camera Mode
	public static Vector3 CAMERA_THIRD_PERSON_POSITION = new Vector3( 0.0f , 7.0f , -1.0f );
	public static Vector3 CAMERA_THIRD_PERSON_ANGLE = new Vector3( 80.0f , 0.0f , 0.0f );
	public static Vector3 CAMERA_FIRST_PERSON_POSITION = new Vector3( 0.0f , 1.0f , 0.0f );
	public static Vector3 CAMERA_FIRST_PERSON_ANGLE = new Vector3( 10.0f , 0.0f , 0.0f );

}
