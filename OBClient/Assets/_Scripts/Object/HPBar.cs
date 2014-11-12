using UnityEngine;
using System.Collections;

public class HPBar : MonoBehaviour
{
	private Camera nguiCamera;
	private GameObject followingMob = null;
	public UnityEngine.GameObject FollowingMob
	{
		set { followingMob = value; }
	}

	private Mob followingMobInfo;
	private UISlider hpValueUI;

	void Awake()
	{
		nguiCamera = GameObject.FindGameObjectWithTag( "NGUICamera" ).camera;
	}

	public void InitHpBar(GameObject followingMob)
	{
		this.followingMob = followingMob;
		gameObject.SetActive( true );
	}

	void OnEnable()
	{
		followingMobInfo = followingMob.GetComponent<Mob>();
		hpValueUI = gameObject.GetComponent<UISlider>();
	}
	
	void Update()
	{
		// set position in scene
		Vector3 characterPosition = Camera.main.WorldToViewportPoint( followingMob.transform.position );
		Vector3 menuPosition = nguiCamera.ViewportToWorldPoint( characterPosition );
		transform.position = new Vector3( menuPosition.x , menuPosition.y - 0.1f, 0.0f );

		// set value of hp
		hpValueUI.value = followingMobInfo.CurrentStat.hp / followingMobInfo.EnemyStat.hp;
	}

	void OnDeactivate()
	{
		followingMob = null;
	}
}
