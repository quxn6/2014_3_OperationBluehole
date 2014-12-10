using UnityEngine;
using System.Collections;

public class SWarrior : MonoBehaviour , IAnimatable
{
	private Animator animator;
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

	public void PlayAttack()
	{
		animator.SetBool( "walk" , false );
		animator.SetTrigger( "skill_0" );
	}

	public void PlayDead()
	{
		animator.SetBool( "walk" , false );
		animator.SetTrigger( "death" );
	}

	public void PlayHit()
	{
		animator.SetTrigger( "damageLeft" );
	}

	public void PlaySkill_0() { animator.SetTrigger( "skill_0" ); }
	public void PlaySkill_1() { animator.SetTrigger( "skill_1" ); }
	public void PlaySkill_2() { animator.SetTrigger( "skill_2" ); }
	public void PlayBuff_0() { animator.SetTrigger( "buff_0" ); }
	public void PlayBuff_1() { animator.SetTrigger( "buff_1" ); }
	public void PlayBuff_2() { animator.SetTrigger( "buff_2" ); }
	public void PlayHooray() { animator.SetTrigger( "hooray" ); }
}
