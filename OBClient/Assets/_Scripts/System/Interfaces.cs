using UnityEngine;
using System.Collections;

public interface IAnimatable
{
	void PlayIdle();
	void PlayWalk();
	void PlayDead();
 	void PlayHit();
	void PlaySkill_0();
	void PlaySkill_1();
	void PlaySkill_2();
	void PlayBuff_0();
	void PlayBuff_1();
	void PlayBuff_2();
 	void PlayHooray();
}

