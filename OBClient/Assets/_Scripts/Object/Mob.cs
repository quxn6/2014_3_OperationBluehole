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
			0,
			enemyStat.actualParams[(int)OperationBluehole.Content.ParamType.spRegn],
			enemyStat.baseStats[(int)OperationBluehole.Content.StatType.Lev]);
	}

	public IEnumerator Attack()
	{
		// play attack animation
		animatable.PlayWalk();
		yield return new WaitForSeconds( GameConfig.MOB_ATTACKMOVING_TIME );
		animatable.PlayAttack();

		// after animation over, accept damage to hero
		while ( IsAnimationPlaying() )
		{
			yield return null;
		}
		animatable.PlayIdle();
	}

	// At proper data type apply positive value
	public void UpdateCharacterData( OperationBluehole.Content.GaugeType dataType, float value )
	{
		switch ( dataType )
		{
			case OperationBluehole.Content.GaugeType.Hp:
				mobData.currentHp += value;				
				break;
			case OperationBluehole.Content.GaugeType.Mp:
				mobData.currentMp += value;
				break;
			case OperationBluehole.Content.GaugeType.Sp:
				mobData.sp += value;				
				break;
		}
		
		if ( mobData.currentHp <= 0	)
		{
			BeKilled();
		}
	}

	public void BeAttacked()
	{
		// play hit or something
		//animatable.playHit();
	}

	public void BeKilled()
	{
		animatable.PlayDead();
	}

	public bool IsAnimationPlaying()
	{
		return animation.isPlaying;
	}
}
