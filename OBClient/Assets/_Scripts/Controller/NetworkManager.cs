using UnityEngine;
using System.Collections;


public class NetworkManager : MonoBehaviour 
{
	public GameObject sceneManager;

	private string dungeonMap = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXOOOOOOOOOOOOOOOOOOOOOOOOOOOOOXI               XX     I  M  XOXXXXXXXXXXXXXOOXXXXXXXXXXXXOXXXXX XXXXXXXXXXXXX           XOX           XOOX          XOXXXXX XXXXXXXXXXXXXXXXX XXXXXXXOXXXXX XXXXXXXOOXXXXXXXX XXXOX                       XOOOOOOOOOOOX XOOOOOOOOOOOOOOOX XOOOXI        I      XXXXXX XXXXXXOXXXXXX XXXXXXXXXXXOOOOOX XOOOX    MMM  I      XOX         XOX                 XXXXXX XXXOX                XOXX XXXXXXXXOX             XXXI     M   XOXXXXXXXXXXXXXXXXXXOOX XOOOOOOOOX        II M XOX          XOOOOOOOOOOOOOOOOOOXXXX XXXXXXXXXX             XOXXXXXXXXXXXXOOXXXXXXXXXXXXXXXOX          I XXXXX XXXXXXXXXXOOOOOOOOOOOOOOOX   M      I  XXX        M   XXXXX XXXXXXXXXXXXXXXXXXXXXXXXOX                I M         XXI        XX    M  I        XOXXXXXXXXXXXX XXXX            XX         XX                XOOOOOOOOOOOOX XOOXXXXXXXX XXXXXX      M  XXXXXXXXXXXXXXXX XXXXXXXXXXXXXXX XXXXXXXXXXX XXX               XXXXXXXXXXXXX XXXI         I    XX            XXXXXX XXXXXX      I         XX    M          XX   M        XOOOOX XOOOOX     M       M  XX               XX  M    I    XOXXXX XXXXOXI           M   XX         I  M  XX    I I     XX        XOXI        I    M XX               XX            XX XXXXXXXXOX                XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX XOOOOOOOOXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX XXXXXXXXXXXXXXXXXXXXXXXXXXXX            P   XX       I         XX      XX        I    XX    I   I  M        I              XX   M  XX  I   M      XX                XX     M   I       XX      XX       M     XXXXXXXXXX XXXXXXXXXM   I      M     XX      XX             XXXXXXXXXX XXXXXXXXX   I             XX  I   XX I      M    XX          I M   XXM                XX      XX             XX            MI  XX                 XX XXXXXXXXXXXXX XXXXXXXX   MI      M    XXXXXXXXX XXXXXXXXXXX XXXXXXOOOOOOX XOOOOOOX       I I   I  XOOOOOOOX XOOOOOOOOOX    I XOXXXXXX XXXXXXOX   M            XOXXXXXXX XXXXXXXXXOXI     XOXI          XOX      M         XOX  M         I  XOXM     XOX    M      XOX                XOX               XOX      XOX M         XOX                XOXXXXXXXXXXXXXXXXXOX      XOX        I  XOXXXXXXXXXXXXX XXXXOOOOOOOOOOOOOOOOOOOX   I  XXX I  I      XOOOOOOOOOOOOOX XOOOOOOOOOOOOOOOOOOOOOOX                    XOOXXXXXXXXXXXX XXXXXXXXXXXXXXXXXXXXXXOX      XXXI          XOOX         XX X     M              XOX      XOX           XOOX I       XX X                    XOXM     XOX      I    XOOX M I     XX X                    XOX      XOX           XOOX         XX X       I I          XOX  IM  XOX  M     M  XOOX         XX X  M   I  I      M   XOX M    XOX           XOOX I M              M        MI    XOX      XOXXXXXXXXXXXXXOOX       I XXXX       M     I      XOXXXXX XXOOOOOOOOOOOOOOOOX         XOOX M     I   MI   I   XOXXXXX XXXXXXXXXXXXXXXXXOX     M   XOOX                    XOX  I     XX           XOX         XOOXXXXXXXXXXXXXXX XXXXXXXX  M     XX I       M XOXXXXXXX XXXOOOOOOOOOOOOOOOOX                 XX           XOOOOOOOX XOOOXXXXXXXXXXXXXXXX XXXXXXXXXXXX XXXXXXXXX XXXXXXXXXXXXXXX XXXXX               M      XXXXXX XX    XXX XXXXXXXX        M  XX   M I         M I    XX     I  XX   M  I    XX      M    XX      I        M      XX        XX        I  XX    M      XX        I             XX    M   XXM          XXI        I XX          M           XX  I     XX M         XX    I      XXI                I    XX        XX           XX           XX        I    MM  I    XX        XX   I       XX           XX                      XX        XX           XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
	private int size = 60;

	static private NetworkManager instance;
	static public NetworkManager Instance
	{
		get { return instance; }		
	}

	void Awake()
	{
		instance = this;
	}

	public void RequestMapInfo()
	{
		StartCoroutine(DummyMapInfoResponse());
	}
	
	IEnumerator DummyMapInfoResponse()
	{
		yield return new WaitForSeconds( 0.1f );
		HandleMapInfo();
	}

	public void HandleMapInfo()
	{
		sceneManager.GetComponent<Loading>().LoadMap( new Dungeon( dungeonMap , size ) );
	}
}
