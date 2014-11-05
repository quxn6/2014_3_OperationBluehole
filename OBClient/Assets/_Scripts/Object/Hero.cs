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

	void Awake()
	{
		faceUI = GameObject.Find( gameObject.name +"Icon BG/Face" );
		hpUI = GameObject.Find( gameObject.name + "Icon BG/HP" );
		mpUI = GameObject.Find( gameObject.name + "Icon BG/MP" );
		levelUI = GameObject.Find( gameObject.name + "Level/Label" );
		//levelUI.GetComponent<UILabel>().text = gameObject.name;
	}

	// Set hero UI value with character data
	public void InitHeroUI()
	{

	}
}
