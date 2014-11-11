using UnityEngine;
using System.Collections;

public enum MobType : uint
{
	Dummy = 0 ,
	Demon = 1 ,
	Troll = 2 ,
	Goblin = 3 ,
	IceGolem = 4 ,
	Spider = 5 ,
	Skeleton_Warrior = 6 ,
	Skeleton_Soldier = 7 ,
	Skeleton_Mage = 8 ,
	Length = 9,
}

static class GameConfig
{
	/////////////// test data////////////////////////////////////

	// test Enemy spec
	public static int DUMMY_ENEMY_COUNT = 200;
	public static int MOB_SPIDER_GROUP_COUNT = 4;
	public static float MOB_DUMMY_HP = 300.0f;
	public static float MOB_DUMMY_MP = 100.0f;
	public static float MOB_DUMMY_SP = 100.0f;

	/////////////////////////////////////////////////////////////

	// battle
	public const int MAXIMUM_HERO_COUNT = 4;

	// Scene
	public const float SPLASH_IMAGE_PLAY_TIME = 2.0f;

	// In Dungeon Constants
	public const float MINIMAL_UNIT_SIZE = 1.0f;
	public const float CHARACTER_MOVING_TIME = 0.5f;
	public const float CHARACTER_TURNING_TIME = 0.2f;
	public const float CHARACTER_ROTATE_DEGREE = 90.0f;

	// Camera Mode
	public static Vector3 CAMERA_THIRD_PERSON_POSITION = new Vector3( 0.0f , 7.0f , -1.0f );
	public static Vector3 CAMERA_THIRD_PERSON_ANGLE = new Vector3( 80.0f , 0.0f , 0.0f );
	public static Vector3 CAMERA_FIRST_PERSON_POSITION = new Vector3( 0.0f , 1.0f , 0.0f );
	public static Vector3 CAMERA_FIRST_PERSON_ANGLE = new Vector3( 10.0f , 0.0f , 0.0f );
}
