using UnityEngine;
using System.Collections;

public class SimulationResult : MonoBehaviour
{
	private BoxCollider buttonCollider = null;
	void Awake()
	{
		buttonCollider = gameObject.GetComponent<BoxCollider>();
	}

	void Update()
	{
		buttonCollider.enabled = ( NetworkManager.Instance.HasResult ) ? true : false;
	}
}
