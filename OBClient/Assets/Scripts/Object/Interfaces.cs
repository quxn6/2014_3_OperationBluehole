using UnityEngine;
using System.Collections;

public interface IAnimatorable
{
	void PlayIdle();
	void PlayWalk();
	void PlayAttack();
	void PlayDead();
}

