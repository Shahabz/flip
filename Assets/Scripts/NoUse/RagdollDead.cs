using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollDead : MonoBehaviour {
	[SerializeField]
	Transform[] ragdolls;

	static RagdollDead instance;
	public static RagdollDead Instance{
		get { return instance; }
	}   
	private void Awake()
	{
		instance = this;
	}

	void Update(){
//		if (Input.GetKeyDown (KeyCode.A)) {
//			ChangeToDead ();
//		}
	}

	//开启布娃娃
	public void ChangeToDead(){
		foreach (Transform trans in ragdolls) {
			Collider coll = trans.GetComponent<Collider> ();
			Rigidbody rig = trans.GetComponent<Rigidbody> ();
			if(coll)
				coll.enabled = true;
			rig.isKinematic = false;
			rig.useGravity = true;
			transform.GetComponent<Animator> ().enabled = false;
		}
	}

	void ChangeToLive(){
		foreach (Transform trans in ragdolls) {
			Collider coll = trans.GetComponent<Collider> ();
			Rigidbody rig = trans.GetComponent<Rigidbody> ();
			if(coll)
				coll.enabled = false;
			rig.isKinematic = true;
			rig.useGravity = false;
			transform.GetComponent<Animator> ().enabled = true;
		}
	}
}
