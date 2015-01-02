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

	public void SetStatus()
	{
		// Set item
		for ( int i = 0 ; i < DataManager.Instance.clientPlayerData.Consumable.Count ; ++i)
		{
			itemIcons[i].SetActive( true );
			itemIcons[i].GetComponent<UISprite>().spriteName = itemAtlas.spriteList[i].name;
		}

		// Set skill


		// Set equip
	}
}
