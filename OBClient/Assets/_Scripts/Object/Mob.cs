using UnityEngine;
using System.Collections;

public class Mob : MonoBehaviour
{
	private int mobId = -1;
	public int MobId
	{
		get { return mobId; }
		set { mobId = value; }
	}

	private CharacterData enemyStat;	public CharacterData EnemyStat
	{
		get { return enemyStat; }
	}
	private CharacterData currentStat;	public CharacterData CurrentStat
	{
		get { return currentStat; }
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

	public void InitMob(CharacterData enemyStat)
	{
		this.enemyStat = enemyStat;
		currentStat = enemyStat;
	}

	public void BeAttacked( float damage )
	{
		currentStat.hp -= damage;
	}

	public bool IsAnimationPlaying()
	{
		return animation.isPlaying;
	}
}
