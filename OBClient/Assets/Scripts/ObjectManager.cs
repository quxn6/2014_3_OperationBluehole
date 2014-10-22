using UnityEngine;
using System.Collections;

public class ObjectManager : MonoBehaviour
{
	static private ObjectManager instance;
	static public ObjectManager Instance
	{
		get { return instance; }		
	}
	
	public GameObject macaronObject;
	public GameObject passiveObject;
	public Sprite[] passiveObjectSprites;
	private HYObjectPool activeObjectPool;
	public HYObjectPool ActiveObjectPool
	{
		get { return activeObjectPool; }
	}
	private HYObjectPool passiveObjectPool;
	public HYObjectPool PassiveObjectPool
	{
		get { return passiveObjectPool; }
	}

	void Awake()
	{
		instance = this;
		activeObjectPool = ScriptableObject.CreateInstance<HYObjectPool>();
		passiveObjectPool = ScriptableObject.CreateInstance<HYObjectPool>();
		activeObjectPool.InitPool( macaronObject , GameConfig.NUMBER_OF_ACTIVE_OBJECT );
		passiveObjectPool.InitPool( passiveObject , GameConfig.NUMBER_OF_PASSIVE_OBJECT );
	}

	void OnEnable()
	{
		ResetObjects();
	}

	public void ResetObjects()
	{		
		// initialize macarons
		activeObjectPool.ResetPool();
		for ( int i = 0 ; i < GameConfig.NUMBER_OF_ACTIVE_OBJECT ; ++i )
		{
			GameObject macaronInstance = activeObjectPool.PullObject();
			Vector3 newPosition = new Vector3(
				Random.Range( GameConfig.DEFAULT_GAUGE_BEG_X_POS , GameConfig.DEFAULT_GAUGE_END_X_POS ) ,
				Random.Range( 0.0f , GameConfig.HEIGHT_WEIGHT )
				);
			macaronInstance.transform.position = newPosition;
		}

		// initialize background object
		passiveObjectPool.ResetPool();
		for ( int j = 0 ; j < GameConfig.NUMBER_OF_PASSIVE_OBJECT ; ++j )
		{
			GameObject backgroundInstance = passiveObjectPool.PullObject();
			Vector3 newPosition = new Vector3(
				Random.Range( GameConfig.DEFAULT_GAUGE_BEG_X_POS , GameConfig.DEFAULT_GAUGE_END_X_POS ) ,
				Random.Range( 0.0f , GameConfig.HEIGHT_WEIGHT )
				);
			backgroundInstance.transform.position = newPosition;
			backgroundInstance.GetComponent<SpriteRenderer>().sprite = passiveObjectSprites[Random.Range( 0 , passiveObjectSprites.Length )];
		}
	}
}
