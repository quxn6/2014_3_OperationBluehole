using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
		NetworkManager.Instance.RequestMapInfo();
	}

}
