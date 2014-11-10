using UnityEngine;
using System.Collections;

public class Troll : MonoBehaviour , IAnimatable
{
	public void PlayIdle()
	{
		animation.CrossFade( "Idle_01" );
	}

	public void PlayWalk()
	{
		animation.CrossFade( "Run" );
	}

	public void PlayAttack()
	{
		if ( 0 == Random.Range( 0 , 2 ) % 2 )
		{
			animation.CrossFade( "Attack_01" );
		}
		else
		{
			animation.CrossFade( "Attack_02" );
		}
	}

	public void PlayDead()
	{
		animation.CrossFade( "Die" );
	}
}
