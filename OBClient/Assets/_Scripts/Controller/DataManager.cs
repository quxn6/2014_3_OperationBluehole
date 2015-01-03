using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct CharacterData
{
	public float maxHp;
	public float maxMp;
	public float currentHp;
	public float currentMp;
	public float sp;
	public float spRegen;
	public int level;

	public CharacterData( float hp , float mp , float sp , float spRegen, int level)
	{
		this.maxHp = hp;
		this.maxMp = mp;
		this.currentHp = hp;
		this.currentMp = mp;
		this.sp = sp;
		this.spRegen = spRegen;
		this.level = level;
	}
}

// manage raw data that had received from server in json form
public class DataManager : MonoBehaviour
{
	public UIAtlas atlasSet = null;

	// real data
	public OperationBluehole.Content.Party UserParty { get; private set; }
	public List<OperationBluehole.Content.Party> MobPartyList{ get; private set; }
	public List<OperationBluehole.Content.Item> ItemList{ get; private set; }

	public List<OperationBluehole.Content.Party> EncounteredMobPartyList { get; set; }
	public List<OperationBluehole.Content.Item> LootedItemList { get; set; }

	public NetworkManager.ClientPlayerData clientPlayerData { get; set; }
	//Warning!! For now, user replay only 1 result
	public NetworkManager.SimulationResult latestSimulationResult { get; set; }

	static private DataManager instance = null;
	static public DataManager Instance
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
		EncounteredMobPartyList = new List<OperationBluehole.Content.Party>();
	}

	public void SetReplayMapData(OperationBluehole.Content.Party userParty, List<OperationBluehole.Content.Party> mobPartyList, List<OperationBluehole.Content.Item> itemList)
	{
		this.UserParty = userParty;
		this.MobPartyList = mobPartyList;
		this.ItemList = itemList;
	}
}
