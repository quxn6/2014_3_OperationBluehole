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
		animator.SetBool( "walk", true );
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
}
