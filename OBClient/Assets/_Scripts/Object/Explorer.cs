using UnityEngine;
using System.Collections;

public class Explorer : MonoBehaviour, IAnimatable
{
	private Animator animator;
	// Use this for initialization
	void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void PlayIdle()
	{
		animator.SetTrigger( "Idle" );		
	}

	public void PlayWalk()
	{
		animator.SetTrigger( "Walk" );		
	}

	public void PlayAttack()
	{
		animator.SetTrigger( "Attack" );		
	}

	public void PlayDead()
	{
		animator.SetTrigger( "Jump" );		
	}
}
