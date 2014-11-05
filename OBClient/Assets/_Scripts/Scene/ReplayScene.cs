using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReplayScene : MonoBehaviour 
{
	private GameObject playerParty = null;

	void Awake()
	{
		playerParty = MapManager.Instance.PlayerParty;
	}
}
