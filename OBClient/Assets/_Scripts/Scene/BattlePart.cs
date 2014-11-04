using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattlePart : MonoBehaviour 
{
	public GameObject playerPartyStatus = null;

	private GameObject playerParty = null;
	public void InitBattlePart()
	{
		playerParty = MapManager.Instance.PlayerParty;
		EnvironmentManager.Instance.PutCamera( playerParty , CameraMode.FIRST_PERSON );
	}
}
