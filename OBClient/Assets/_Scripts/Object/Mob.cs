using UnityEngine;
using System.Collections;

public class Mob : MonoBehaviour
{
	private CharacterData mobData;
	public CharacterData MobData
	{
		get { return mobData; }	
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

	public void InitMobData(OperationBluehole.Content.Mob enemyStat)
	{
		mobData = new CharacterData(
			enemyStat.actualParams[(int)OperationBluehole.Content.ParamType.maxHp] ,
			enemyStat.actualParams[(int)OperationBluehole.Content.ParamType.maxMp] ,
			enemyStat.baseStats[(int)OperationBluehole.Content.StatType.Lev] ,
			0 );
	}

	public void BeAttacked( float damage )
	{
		mobData.currentHp -= damage;
	}

	public bool IsAnimationPlaying()
	{
		return animation.isPlaying;
	}
}
