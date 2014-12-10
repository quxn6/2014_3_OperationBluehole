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

	public void PlayHit() { }
	public void PlaySkill_0() { }
	public void PlaySkill_1() { }
	public void PlaySkill_2() { }
	public void PlayBuff_0() { }
	public void PlayBuff_1() { }
	public void PlayBuff_2() { }
	public void PlayHooray() { }
}
