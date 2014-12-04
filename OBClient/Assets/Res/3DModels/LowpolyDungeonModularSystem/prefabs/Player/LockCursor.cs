using UnityEngine;
using System.Collections;

public class LockCursor : MonoBehaviour {
	bool locked = false;

	void DoLockCursor() {
		Debug.Log("Locking cursor");
		Screen.lockCursor = true;
		locked = true;
	}
	void DoUnlockCursor() {
		Debug.Log("Unlocking cursor");
		Screen.lockCursor = false;
		locked = false;
	}
	
	void Update() {
		if(Input.GetButtonDown("Fire1")){
			if(locked == false){
				DoLockCursor();
			}
		}
		if (Input.GetKeyDown("escape"))
			if(locked == true){
				DoUnlockCursor();
			}
		}
	}