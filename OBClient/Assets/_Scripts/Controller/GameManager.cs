using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	void Awake()
	{
		DontDestroyOnLoad( gameObject );
	}
}
