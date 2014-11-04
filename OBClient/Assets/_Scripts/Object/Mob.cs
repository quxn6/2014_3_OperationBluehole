using UnityEngine;
using System.Collections;

public class Mob : MonoBehaviour , IAnimatable{

	// Use this for initialization
	void Start () {
	
	}
	 
	// Update is called once per frame
	void Update () {
	
	}

	public void PlayIdle()
	{
		animation.Play( "idle" );
	}

	public void PlayWalk()
	{

	}

	public void PlayAttack()
	{

	}

	public void PlayDead()
	{

	}
	
}
