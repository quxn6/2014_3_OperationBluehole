using UnityEngine;
using System.Collections;

public class ZombieBasic : MonoBehaviour, IAnimatable {

	protected Animator animator;

	void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void PlayIdle()
	{
		//animator.SetTrigger( "Idle" );
		animator.SetBool( "Walk" , false );
	}

	public void PlayWalk()
	{
		animator.SetBool( "Walk" , true );
	}

	public void PlayDead()
	{
		animator.SetBool( "Walk" , false );
		animator.SetTrigger( "Death" );
	}

	public void PlayHit()
	{
		int rand = Random.Range( 0 , 3 ) % 3;
		if ( rand == 0 )
		{
			animator.SetTrigger( "HitRight" );
		}
		else if ( rand == 1 )
		{
			animator.SetTrigger( "HitLeft" );
		}
		else
		{
			animator.SetTrigger( "HitFront" );
		}

	}

	public void PlaySkill_0() { animator.SetTrigger( "Melee0" ); }
	public void PlaySkill_1() { animator.SetTrigger( "Melee1" ); }
	public void PlaySkill_2() { animator.SetTrigger( "Melee2" ); }
	public void PlayBuff_0() { animator.SetTrigger( "Buff0" ); }
	public void PlayBuff_1() { animator.SetTrigger( "Buff1" ); }
	public void PlayBuff_2() { animator.SetTrigger( "Buff2" ); }
	public void PlayHooray() { animator.SetTrigger( "Jump" ); }
}
