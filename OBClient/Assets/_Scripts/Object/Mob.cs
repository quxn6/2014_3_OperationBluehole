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
	private Animator anim;
	private delegate void PlaySkill();
	PlaySkill[] actionSkill = null;
	PlaySkill[] buffSkill = null;

	void Awake()
	{
		animatable = (IAnimatable)GetComponent( typeof( IAnimatable ) );
		anim = GetComponent<Animator>();
		buffSkill = new PlaySkill[3];
		actionSkill = new PlaySkill[3];
		actionSkill[0] = animatable.PlaySkill_0;
		actionSkill[1] = animatable.PlaySkill_1;
		actionSkill[2] = animatable.PlaySkill_2;
		buffSkill[0] = animatable.PlayBuff_0;
		buffSkill[1] = animatable.PlayBuff_1;
		buffSkill[2] = animatable.PlayBuff_2;
	}

	void OnEnable()
	{
		//animatable.PlayIdle();
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

	public IEnumerator Attack(OperationBluehole.Content.TurnInfo turnInfo)
	{
		// play attack animation
		//animatable.PlayWalk();
		yield return new WaitForSeconds( GameConfig.MOB_ATTACKMOVING_TIME );
		//animatable.PlayAttack();
//		Debug.Log( "   mob Action start " + turnInfo.srcIdx );

		// if target is one enemy, use melee skills
		if ( turnInfo.targets.Count == 1 && turnInfo.srcType != turnInfo.targets[0].targetType )
		{
			actionSkill[(int)turnInfo.skillId % actionSkill.Length]();
		}
		else
		{
			buffSkill[(int)turnInfo.skillId % actionSkill.Length]();
		}
			
		
		// after animation over, accept damage to hero
		while ( IsAnimationPlaying() )
		{
//			Debug.Log( "      mob Action ~ing " + turnInfo.srcIdx );
			yield return null;
		}
//		Debug.Log( "   mob Action end " + turnInfo.srcIdx );
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
		
		if ( mobData.currentHp <= 0u )
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
		// check mecanim or not
		if ( animation == null )
		{
			return !anim.GetCurrentAnimatorStateInfo( 0 ).IsName( "default" );
		}			
		else
			return animation.isPlaying;
	}
}
