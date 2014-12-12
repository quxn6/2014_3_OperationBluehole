using UnityEngine;
using System.Collections;

public class SkeletonBasic : MonoBehaviour , IAnimatable
{
	protected Animator animator;
	// Use this for initialization
	void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void PlayIdle()
	{
		//animator.SetTrigger( "Idle" );
		animator.SetBool( "walk" , false );
	}

	public void PlayWalk()
	{
		animator.SetBool( "walk" , true );
	}

	public void PlayDead()
	{
		animator.SetBool( "walk" , false );
		animator.SetTrigger( "death" );
	}

	public void PlayHit()
	{
		if (Random.Range(0,2) % 2 == 0)
		{
			animator.SetTrigger( "damageLeft" );
		}
		else
		{
			animator.SetTrigger( "damageRight" );
		}
		
	}

	public void PlaySkill_0() { animator.SetTrigger( "ShieldBash" ); }
	public void PlaySkill_1() { animator.SetTrigger( "SwordSlice" ); }
	public void PlaySkill_2() { animator.SetTrigger( "SwordSlash" ); }
	public void PlayBuff_0() { animator.SetTrigger( "buff_0" ); }
	public void PlayBuff_1() { animator.SetTrigger( "buff_1" ); }
	public void PlayBuff_2() { animator.SetTrigger( "buff_2" ); }
	public void PlayHooray() { animator.SetTrigger( "hooray" ); }
}
