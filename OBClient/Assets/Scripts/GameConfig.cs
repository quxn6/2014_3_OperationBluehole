using UnityEngine;
using System.Collections;

public enum InputType
{
	NONE = 0 ,
	SWIPE_EAST = 1 ,
	SWIPE_WEST = 2 ,
	SWIPE_SOUTH = 4 ,
	SWIPE_NORTH = 8 ,
	TAP = 16 ,
}

static class GameConfig
{
	// Variables from Server
	static public float MAX_SCORE_WEIGHT;
	static public float WEIGHT_LOG_BASE;
	static public float GRAVITY;
	static public float AIR_AGIANST;
	static public float MAX_MACARON_DENSITY;
// 	static public float HEIGHT_WEIGHT;
// 	static public float DEFAULT_HEIGHT;
	//static public void InitPhysics( float maxScoreWeight , float weightLogBase , float gravity , float airAgainst , float maxMacaronDensity , float heightWeight )
	static public bool InitPhysics()
	{
		if ( !PlayerPrefs.HasKey("maxScoreWeight") )
		{
			Debug.LogError( "ERROR : server variables aren't loaded yet" );
			return false;
		}
		MAX_SCORE_WEIGHT = PlayerPrefs.GetFloat("maxScoreWeight");
		WEIGHT_LOG_BASE = PlayerPrefs.GetFloat("weightLogBase");
		GRAVITY = PlayerPrefs.GetFloat("gravity");
		AIR_AGIANST = PlayerPrefs.GetFloat("airRegist");
		MAX_MACARON_DENSITY = PlayerPrefs.GetFloat("maxMacaronDensity");
// 		HEIGHT_WEIGHT = PlayerPrefs.GetFloat( "heightWeight" );
// 		DEFAULT_HEIGHT = HEIGHT_WEIGHT / 2;
		return true;
	}

	// Jump Power Gauge
	public const float GAUGE_MOVING_TIME = 1.5f;
	public const float HEIGHT_WEIGHT = 200.0F;
	public const float DEFAULT_HEIGHT = HEIGHT_WEIGHT / 2;
	public const float DEFAULT_GAUGE_BEG_X_POS = -3.0f;
	public const float DEFAULT_GAUGE_MID_X_POS = 0.0f;
	public const float DEFAULT_GAUGE_END_X_POS = 3.0f;

	// Umbrella Girl
	public const float JUMP_TIME = 5.0F;


	// camera
	public const float CAMERA_POSITION_READY = 4.0f;
	public const float CAMERA_POSITION_FREEFALL = -CAMERA_POSITION_READY;
	public const float CAMERA_POSITION_OPENPARACHUTE = 0.0f;

	// System
	public const float SWIPE_TIME = 0.15f;
	public const float PARACHUTE_OPEN_TIME = 3.0f;

	// Object
	public const int NUMBER_OF_ACTIVE_OBJECT = 30;
	public const int NUMBER_OF_PASSIVE_OBJECT = 20;
	public const int PASSIVE_OBJECTS_MINIMUM = 3;
	public const int PASSIVE_OBJECTS_MAXIMUM = 7;

	// Splash Image
	public const float SPLASH_IMAGE_PLAY_TIME = 1.5f;
}
