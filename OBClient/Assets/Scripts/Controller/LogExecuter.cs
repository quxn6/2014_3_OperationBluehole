using UnityEngine;
using System.Collections;

public enum MoveDirection
{
	UP,
	DOWN,
	LEFT,
	RIGHT,
}

public class LogExecuter : MonoBehaviour
{
	private GameObject playerParty = null;

	void Awake()
	{
		playerParty = GameObject.FindGameObjectWithTag( "Player" );		
	}

	void MoveCharacter(GameObject character, MoveDirection direction)
	{

	}

	void StartBattle(GameObject target)
	{

	}

}
