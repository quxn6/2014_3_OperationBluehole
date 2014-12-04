using UnityEngine;
using System.Collections;

public class ShadowBlob : MonoBehaviour {

	public GameObject ShadowGO;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update() {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit))
		{
			float distanceToGround = hit.distance;
			Vector3 positionOnGround = hit.point;
			Debug.Log(positionOnGround);
			Vector3 temp = ShadowGO.transform.localScale;
			float distanceToGround2 = 1.5f + distanceToGround*0.2f;
			temp.x = distanceToGround2;
			temp.y = distanceToGround2;
			positionOnGround.y += 0.1f;
			ShadowGO.transform.position = positionOnGround;
			ShadowGO.transform.localScale = temp;
		}
		
	}
}
