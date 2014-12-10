using UnityEngine;
using System.Collections;

public class SkeletonMage : MonoBehaviour , IAnimatable
{
	public void PlayIdle()
	{
		animation.Blend( "waitingforbattle" );
	}

	public void PlayWalk()
	{
		animation.CrossFade( "run" );
	}

	public void PlayAttack()
	{
		animation.CrossFade( "attack" );
	}

	public void PlayDead()
	{
		animation.CrossFade( "die" );
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
