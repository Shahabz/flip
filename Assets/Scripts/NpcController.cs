using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour {
	Animator animator;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
	}

	void OnTriggerEnter(Collider coll){
		if (coll.tag == "NpcTrigger") {			
			//animator.SetFloat ("Speed_f", -animator.GetFloat ("Speed_f"));
			transform.eulerAngles = new Vector3(transform.eulerAngles.x,-transform.eulerAngles.y,transform.eulerAngles.z);
		}
	}



}
