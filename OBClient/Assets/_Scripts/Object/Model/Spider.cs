using UnityEngine;
using System.Collections;

public class Spider : MonoBehaviour , IAnimatable
{
	public void PlayIdle()
	{
		animation.CrossFade( "iddle" );
	}

	public void PlayWalk()
	{
		animation.CrossFade( "walk" );
	}

	public void PlayAttack()
	{
		switch ( Random.Range( 0 , 3 ) % 3 )
		{
			case 0:
				animation.CrossFade( "attack_Melee" );
				break;
			case 1:
				animation.CrossFade( "attack_Melee2" );
				break;
			case 2:
				animation.CrossFade( "attack_leap" );
				break;
			default:
				animation.CrossFade( "attack_Melee" );
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
