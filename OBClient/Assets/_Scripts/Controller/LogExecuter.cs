using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MoveDirection
{
	STAY = 0 ,
	LEFT = 1 ,
	DOWN = 2 ,
	RIGHT = 3 ,
	UP = 4 ,
}

//delegate void CharacterController(GameObject character);

public class LogExecuter : MonoBehaviour
{
	private GameObject playerParty = null;
	private List<GameObject> mobList = null;

	static private LogExecuter instance;
	static public LogExecuter Instance
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
		mobList = new List<GameObject>();
	}

	// init variables
	public void InitLogExecuter()
	{
		playerParty = MapManager.Instance.PlayerParty;
		if ( playerParty == null )
			Debug.LogError( "Error (Log Executer) : There is No Player Party" );

		mobList = MapManager.Instance.MobList;
		if ( mobList == null )
			Debug.LogError( "Error (Log Executer) : There is No Mob List" );
	}

	// Warning!!! Dirty Code. We should use coroutine here
	public void MoveCharacter( GameObject character , MoveDirection direction )
	{
		IAnimatable animatable = (IAnimatable)character.GetComponent( typeof( IAnimatable ) );
		animatable.PlayWalk();

		//character.transform.rotation = Quaternion.Euler( 0.0f , 90.0f * (float)direction , 0 );
		//character.transform.Translate( 0.0f , 0.0f , 1.0f , Space.Self );

		// Move After Rotation
		iTween.RotateTo( character , iTween.Hash(
			"y" , GameConfig.CHARACTER_ROTATE_DEGREE * (float)direction ,
			"time" , GameConfig.CHARACTER_MOVING_TIME ,
			"islocal" , true ,
			"easetype" , iTween.EaseType.linear ,
			"oncomplete" , "MoveOneBlock" ,
			"oncompletetarget" , gameObject ,
			"oncompleteparams" , character
			) );
	}

	private void MoveOneBlock( GameObject character )
	{
		iTween.MoveAdd( character , iTween.Hash(
				"z" , GameConfig.MINIMAL_UNIT_SIZE ,
				"time" , GameConfig.CHARACTER_MOVING_TIME ,
				"islocal" , true ,
				"easetype" , iTween.EaseType.linear
				) );
	}

	public void DoBattle( int targetMobId )
	{
		// Debug.Log( "Start battle!! with" + mob );
		BattleManager.Instance.StartBattle( targetMobId );
	}

	public void MobAttackHero( int mobNumber , int heroNumber , float damage )
	{	
		StopAllCoroutines();
		StartCoroutine( MobAttackProcess( 
			BattleManager.Instance.EnemyInstanceList[mobNumber],
			BattleManager.Instance.heroStatus[heroNumber],
			damage
			) );

		// heroes ui get damage( on UI )
	}

	IEnumerator MobAttackProcess( GameObject mob, GameObject hero, float damage )
	{
		IAnimatable animatableMob = ( (IAnimatable)mob.GetComponent( typeof( IAnimatable ) ) );

		// play attack animation
		animatableMob.PlayWalk();
		yield return new WaitForSeconds(GameConfig.MOB_ATTACKMOVING_TIME);
		animatableMob.PlayAttack();		

		// after animation over, accept damage to hero
		while ( mob.GetComponent<Mob>().IsAnimationPlaying() )
		{
			yield return null;
		}
		
		hero.GetComponent<Hero>().BeAttacked( damage );
		animatableMob.PlayIdle();
	}

	public void HeroAttackMob( int mobNumber , int heroNumber , float damage )
	{
		BattleManager.Instance.EnemyInstanceList[mobNumber].GetComponent<Mob>().BeAttacked( damage );
	}

	public void KillPlayer( int heroesNumber )
	{
		BattleManager.Instance.heroStatus[heroesNumber].GetComponent<Hero>().BeKilled();
	}

	public void KillMob( int mobNumber )
	{
		( (IAnimatable)BattleManager.Instance.EnemyInstanceList[mobNumber].GetComponent( typeof( IAnimatable ) ) ).PlayDead();
	}

}
