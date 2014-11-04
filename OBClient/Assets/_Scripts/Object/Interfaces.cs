using UnityEngine;
using System.Collections;

public interface IAnimatable
{
	void PlayIdle();
	void PlayWalk();
	void PlayAttack();
	void PlayDead();
}

