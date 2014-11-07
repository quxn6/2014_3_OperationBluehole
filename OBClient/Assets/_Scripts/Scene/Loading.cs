using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour
{
	public GameObject loadingUI;
	public GameObject replayButton;
		
	void Start()
	{
		replayButton.SetActive( false );
		loadingUI.GetComponent<TweenAlpha>().enabled = false;		
		NetworkManager.Instance.RequestMapInfo();		
	}

	public void LoadMap(Dungeon dungeonMap)
	{
		MapManager.Instance.InitMapObjects();
		MapManager.Instance.PutMapObjects(dungeonMap);
		replayButton.SetActive( true );
	}

	public void LoadReplayScene()
	{
		loadingUI.GetComponent<TweenAlpha>().enabled = true;
	}

	public void CloseLoadingView()
	{
		gameObject.SetActive( false );
	}
}
