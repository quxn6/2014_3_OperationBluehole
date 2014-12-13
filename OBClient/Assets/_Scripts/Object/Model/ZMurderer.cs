using UnityEngine;
using System.Collections;

public class ZMurderer : ZombieBasic
{
	public void PlaySkill_0() { animator.SetTrigger( "Melee0" ); }
	public void PlaySkill_1() { animator.SetTrigger( "Melee1" ); }
	public void PlaySkill_2() { animator.SetTrigger( "Melee2" ); }
	public void PlayBuff_0() { animator.SetTrigger( "Buff0" ); }
	public void PlayBuff_1() { animator.SetTrigger( "Buff0" ); }
	public void PlayBuff_2() { animator.SetTrigger( "Buff0" ); }
	public void PlayHooray() { animator.SetTrigger( "Jump" ); }
}
