using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour
{
	//[SerializeField]
	private GameObject faceUI = null;
	private GameObject hpUI = null;
	private GameObject mpUI = null;
	private GameObject levelUI = null;

	private CharacterData heroStat;
	public CharacterData HeroStat
	{
		get { return heroStat; }
		set { heroStat = value; }
	}

	private CharacterData currentStat;

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

	public void InitHeroUI()
	{		
		currentStat = heroStat;

		faceUI.GetComponent<UISprite>().spriteName = DataManager.Instance.atlasSet.spriteList[0].name;
		hpUI.GetComponent<UISprite>().fillAmount = currentStat.hp / heroStat.hp;
		mpUI.GetComponent<UISprite>().fillAmount = currentStat.mp / heroStat.mp;
		levelUI.GetComponent<UILabel>().text = heroStat.level.ToString();
	}
}
