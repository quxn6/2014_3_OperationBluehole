using UnityEngine;
using System.Collections;

public class ChibiWarrior : MonoBehaviour , IAnimatable
{
	private Animator animator;

	void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void PlayIdle()
	{
		animation.CrossFade( "BW_Chibi_AttackStandy" );
	}

	public void PlayWalk()
	{
		animation.CrossFade( "BW_Chibi_Run01" );
	}

	public void PlayAttack()
	{
		animation.CrossFade( "BW_Chibi_Attack00" );
	}

	public void PlayDead()
	{
		animation.CrossFade( "BW_Chibi_Death" );
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
