using UnityEngine;
using System.Collections;

public class Golem : MonoBehaviour , IAnimatable
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
		switch ( Random.Range( 0 , 2 ) % 2 )
		{
			case 0:
				animation.CrossFade( "punch" );
				break;
			case 1:
				animation.CrossFade( "hpunch" );
				break;
			default:
				animation.CrossFade( "punch" );
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
