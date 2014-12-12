using UnityEngine;
using System.Collections;

public class SKing : SkeletonBasic
{
	public void PlaySkill_0() { animator.SetTrigger( "TSwordCleave" ); }
	public void PlaySkill_1() { animator.SetTrigger( "SwordSS" ); }
	public void PlaySkill_2() { animator.SetTrigger( "SwordStab" ); }
	public void PlayBuff_0() { animator.SetTrigger( "buff_0" ); }
	public void PlayBuff_1() { animator.SetTrigger( "buff_1" ); }
	public void PlayBuff_2() { animator.SetTrigger( "buff_2" ); }
	public void PlayHooray() { animator.SetTrigger( "hooray" ); }
}
