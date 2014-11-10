using UnityEngine;
using System.Collections;

public class Demon : MonoBehaviour, IAnimatable
{
	public void PlayIdle()
	{
		animation.CrossFade( "idle" );
	}

	public void PlayWalk()
	{
		animation.CrossFade( "walk" );
	}

	public void PlayAttack()
	{
		if ( 0 == Random.Range(0,2) % 2)
		{
			animation.CrossFade( "attack" );
		}
		else
		{
			animation.CrossFade( "attack2" );
		}
	}

	public void PlayDead()
	{
		animation.CrossFade( "death" );
	}
}
