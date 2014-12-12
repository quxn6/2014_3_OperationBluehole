using UnityEngine;
using System.Collections;

public class AnimationTester : MonoBehaviour
{
	public GameObject mob;
	// Use this for initialization
	IEnumerator Start()
	{
		( (IAnimatable)mob.GetComponent( typeof( IAnimatable ) ) ).PlayIdle();
		yield return new WaitForSeconds( 2.0f );

		( (IAnimatable)mob.GetComponent( typeof( IAnimatable ) ) ).PlayWalk();
		yield return new WaitForSeconds( 2.0f );
		
		( (IAnimatable)mob.GetComponent( typeof( IAnimatable ) ) ).PlayDead();
	}

}
