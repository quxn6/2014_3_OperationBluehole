using UnityEngine;
using System.Collections;

public class Spider : MonoBehaviour , IAnimatable
{
	private int modId = -1;
	public int ModId
	{
		get { return modId; }
		set { modId = value; }
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

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
