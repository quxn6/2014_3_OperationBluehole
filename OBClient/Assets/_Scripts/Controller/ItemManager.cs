using UnityEngine;
using System.Collections;

public class ItemManager : MonoBehaviour
{
	public GameObject[] itemIcons;
	public GameObject[] skillIcons;
	public GameObject[] equipIcons;
	public UIAtlas itemAtlas;
	public UIAtlas skillAtlas;
	public UIAtlas equipAtlas;

	static private ItemManager instance = null;
	static public ItemManager Instance
	{
		get { return instance; }
	}

	void Awake()
	{
		if ( null != instance )
		{
			Debug.LogError( this + " already exist" );
			return;
		}

		instance = this;		
	}

	void Start()
	{
		for ( int i = 0 ; i < itemIcons.Length ; ++i)
		{
			itemIcons[i].SetActive( false );
			skillIcons[i].SetActive( false );
			equipIcons[i].SetActive( false );
		}
	}

	public void SetInventoryIcons()
	{
		// Set item
		for ( int i = 0 ; i < DataManager.Instance.clientPlayerData.Consumable.Count ; ++i)
		{
			itemIcons[i].SetActive( true );
			itemIcons[i].GetComponent<UISprite>().atlas = itemAtlas;
			itemIcons[i].GetComponent<UISprite>().spriteName = itemAtlas.spriteList[i].name;
		}

		// Set skill
		for ( int i = 0 ; i < DataManager.Instance.clientPlayerData.Skill.Count ; ++i )
		{
			skillIcons[i].SetActive( true );
			skillIcons[i].GetComponent<UISprite>().atlas = skillAtlas;
			skillIcons[i].GetComponent<UISprite>().spriteName = skillAtlas.spriteList[i].name;
		}

		// Set equip
		for ( int i = 0 ; i < DataManager.Instance.clientPlayerData.Equipment.Count ; ++i )
		{
			equipIcons[i].SetActive( true );
			equipIcons[i].GetComponent<UISprite>().atlas = equipAtlas;
			equipIcons[i].GetComponent<UISprite>().spriteName = equipAtlas.spriteList[i].name;
		}
	}
}
