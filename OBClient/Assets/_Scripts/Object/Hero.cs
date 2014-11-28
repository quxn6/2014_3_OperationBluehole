using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour
{
	//[SerializeField]
	private GameObject faceUI = null;
	private GameObject hpUI = null;
	private GameObject mpUI = null;
	private GameObject spUI = null;
	private GameObject levelUI = null;
	

	private CharacterData heroData;

	void Awake()
	{
		faceUI = GameObject.Find( gameObject.name +"Icon BG/Face" );
		hpUI = GameObject.Find( gameObject.name + "Icon BG/HP" );
		mpUI = GameObject.Find( gameObject.name + "Icon BG/MP" );
		spUI = GameObject.Find( gameObject.name + "Icon BG/Face/SP" );
		levelUI = GameObject.Find( gameObject.name + "Level/Label" );
	}

	// Set hero UI value with character data

	void Start()
	{
		InitHeroUI();
	}

	public void InitHeroData( OperationBluehole.Content.Character characterStat )
	{
		heroData = new CharacterData(
			characterStat.actualParams[(int)OperationBluehole.Content.ParamType.maxHp] ,
			characterStat.actualParams[(int)OperationBluehole.Content.ParamType.maxMp] ,
			0 ,
			characterStat.actualParams[(int)OperationBluehole.Content.ParamType.spRegn] ,
			characterStat.baseStats[(int)OperationBluehole.Content.StatType.Lev] );
	}

	public void InitHeroUI()
	{
		faceUI.GetComponent<UISprite>().color = GameConfig.DEFALUT_HERO_COLOR;
		faceUI.GetComponent<UISprite>().spriteName = DataManager.Instance.atlasSet.spriteList[0].name;
		hpUI.GetComponent<UISprite>().fillAmount = heroData.currentHp / heroData.maxHp;
		mpUI.GetComponent<UISprite>().fillAmount = heroData.currentMp / heroData.maxMp;
		levelUI.GetComponent<UILabel>().text = heroData.level.ToString();
	}

	// At proper data type apply positive value, and update UI
	public void UpdateCharacterData(OperationBluehole.Content.GaugeType dataType, float value)
	{
		switch (dataType)
		{
			case OperationBluehole.Content.GaugeType.Hp:
				heroData.currentHp += value;
				hpUI.GetComponent<UISprite>().fillAmount = heroData.currentHp / heroData.maxHp;
				break;
			case OperationBluehole.Content.GaugeType.Mp:
				heroData.currentMp += value;
				mpUI.GetComponent<UISprite>().fillAmount = heroData.currentMp / heroData.maxMp;
				break;
			case OperationBluehole.Content.GaugeType.Sp:
				heroData.sp += value;
				spUI.GetComponent<UISprite>().fillAmount = heroData.sp / OperationBluehole.Content.Config.MAX_CHARACTER_SP;
				break;
		}
		
		if ( heroData.currentHp <= 0)
		{
			BeKilled();
		}
	}

	public IEnumerator Attack()
	{
		// Some animation for attack
		yield return new WaitForSeconds( 0.1f );
	}

	public void BeAttacked()
	{
		StartCoroutine( ChangeUIColorForSeconds( GameConfig.UI_COLOR_CHANGED_TIME , GameConfig.BEATTACKED_HERO_COLOR ) );
	}

	IEnumerator ChangeUIColorForSeconds(float seconds, Color color)
	{
		faceUI.GetComponent<UISprite>().color = color;
		yield return new WaitForSeconds( seconds );
		faceUI.GetComponent<UISprite>().color = GameConfig.DEFALUT_HERO_COLOR;
	}

	public void BeKilled()
	{
		faceUI.GetComponent<UISprite>().color = GameConfig.DEAD_HERO_COLOR;
	}

}
