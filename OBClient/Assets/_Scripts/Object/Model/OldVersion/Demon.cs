using UnityEngine;
using System.Collections;

public class Demon : MonoBehaviour, IAnimatable
{
	public void PlayIdle()
	{
		animation.Blend( "idle" );
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


	public void PlayHit() { }
	public void PlaySkill_0() { }
	public void PlaySkill_1() { }
	public void PlaySkill_2() { }
	public void PlayBuff_0() { }
	public void PlayBuff_1() { }
	public void PlayBuff_2() { }
	public void PlayHooray() { }
}
