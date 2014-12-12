using UnityEngine;
using System.Collections;

public class Goblin : MonoBehaviour , IAnimatable
{
	public void PlayIdle()
	{
		animation.Blend( "idle" );
	}

	public void PlayWalk()
	{
		animation.CrossFade( "run" );
	}

	public void PlayAttack()
	{
		// for random attack action
		//int actionNumber = Random.Range( 0 , 3 ) % 3;
		switch ( Random.Range( 0 , 3 ) % 3 )
		{
			case 0:
				animation.CrossFade( "attack1" );
				break;
			case 1:
				animation.CrossFade( "attack2" );
				break;
			case 2:
				animation.CrossFade( "attack3" );
				break;
			default:
				animation.CrossFade( "attack1" );
				break;
		}
	}

	public void PlayDead()
	{
		animation.CrossFade( "death" );
	}

	public void PlayHit() { }
	public void PlaySkill_0() { }
	public void PlaySkill_1() { }
	public void PlaySkill_2() { }
	public void PlayBuff_0() { }
	public void PlayBuff_1() { }
	public void PlayBuff_2() { }
	public void PlayHooray() { }
}
