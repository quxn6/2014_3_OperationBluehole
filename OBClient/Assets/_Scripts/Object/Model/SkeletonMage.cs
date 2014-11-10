using UnityEngine;
using System.Collections;

public class SkeletonMage : MonoBehaviour , IAnimatable
{
	public void PlayIdle()
	{
		animation.CrossFade( "waitingforbattle" );
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
}
