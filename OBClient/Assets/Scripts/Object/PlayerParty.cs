using UnityEngine;
using System.Collections;

public class PlayerParty : MonoBehaviour
{
	private Animator animator;
	// Use this for initialization
	void Start()
	{
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void PlayIdle()
	{
		//animator.SetTrigger( "Idle" );		
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
