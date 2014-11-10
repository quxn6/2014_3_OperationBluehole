using UnityEngine;
using System.Collections;

public class Golem : MonoBehaviour , IAnimatable
{
	public void PlayIdle()
	{
		animation.CrossFade( "idle" );
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
}
