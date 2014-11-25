using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour
{
	//[SerializeField]
	private GameObject faceUI = null;
	private GameObject hpUI = null;
	private GameObject mpUI = null;
	private GameObject levelUI = null;

	private CharacterData heroData;

	void Awake()
	{
		faceUI = GameObject.Find( gameObject.name +"Icon BG/Face" );
		hpUI = GameObject.Find( gameObject.name + "Icon BG/HP" );
		mpUI = GameObject.Find( gameObject.name + "Icon BG/MP" );
		levelUI = GameObject.Find( gameObject.name + "Level/Label" );
		//levelUI.GetComponent<UILabel>().text = gameObject.name;
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
			characterStat.baseStats[(int)OperationBluehole.Content.StatType.Lev] ,
			0 );
	}

	public void InitHeroUI()
	{
		faceUI.GetComponent<UISprite>().spriteName = DataManager.Instance.atlasSet.spriteList[0].name;
		hpUI.GetComponent<UISprite>().fillAmount = heroData.currentHp / heroData.maxHp;
		mpUI.GetComponent<UISprite>().fillAmount = heroData.currentMp / heroData.maxMp;
		levelUI.GetComponent<UILabel>().text = heroData.level.ToString();
	}

	public void BeAttacked(float damage)
	{
		heroData.currentHp -= damage;
		hpUI.GetComponent<UISprite>().fillAmount = heroData.currentHp / heroData.currentHp;
	}

	public void BeKilled()
	{
		faceUI.GetComponent<UISprite>().color = GameConfig.DEAD_HERO_COLOR;
	}

}
