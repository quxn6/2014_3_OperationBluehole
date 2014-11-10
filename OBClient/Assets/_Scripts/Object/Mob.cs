using UnityEngine;
using System.Collections;

public class Mob : MonoBehaviour {
	private int mobId = -1;
	public int MobId
	{
		get { return mobId; }
		set { mobId = value; }
	}

	private IAnimatable animatable;

	void Awake()
	{
		animatable = (IAnimatable)GetComponent( typeof( IAnimatable ) );
	}

	void OnEnable()
	{
		animatable.PlayIdle();
	}
}
