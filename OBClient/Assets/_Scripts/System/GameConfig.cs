using UnityEngine;
using System.Collections;

public enum MobType : uint
{
	Dummy = 0 ,
	SkeletonArcher ,
	SkeletonKing ,
	SkeletonMage ,
	SkeletonWarrior ,
	ZombieFatty ,
	ZombieKing ,
	ZombieMurderer ,
	ZombieSnapper ,
	Length ,
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

	// Map
	public const int FLOOR_BLOCK_SIZE = 3;

	// battle
	public const int MAXIMUM_HERO_COUNT = 4;
	public const float MOB_ATTACKMOVING_TIME = 0.4f;

	public static Color DEAD_HERO_COLOR = new Color( 0.2f , 0.2f , 0.2f );
	public static Color BEATTACKED_HERO_COLOR = new Color( 0.7f , 0.2f , 0.2f );
	public static Color DEFALUT_HERO_COLOR = Color.white;
	public const float UI_COLOR_CHANGED_TIME = 0.2f;

	// Scene
	public const float SPLASH_IMAGE_PLAY_TIME = 2.0f;

	// In Dungeon Constants
	public const float MINIMAL_UNIT_SIZE = 1.0f;
	public const float CHARACTER_MOVING_TIME = 0.4f;
	public const float CHARACTER_TURNING_TIME = 0.1f;
	public const float CHARACTER_ROTATE_DEGREE = 90.0f;

	// Camera Mode
	public static Vector3 CAMERA_THIRD_PERSON_POSITION = new Vector3( 0.0f , 7.0f , -1.0f );
	public static Vector3 CAMERA_THIRD_PERSON_ANGLE = new Vector3( 80.0f , 0.0f , 0.0f );
	public static Vector3 CAMERA_FIRST_PERSON_POSITION = new Vector3( 0.0f , 1.0f , 0.0f );
	public static Vector3 CAMERA_FIRST_PERSON_ANGLE = new Vector3( 10.0f , 0.0f , 0.0f );

	// MainMenu
	public const float MENU_SWIPE_TIME = 0.3f;
}
