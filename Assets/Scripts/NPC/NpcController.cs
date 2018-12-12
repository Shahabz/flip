using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour {
	Animator animator;

	void Start () {
		animator = GetComponent<Animator> ();
	}

	//NPC直线往返运动
	void OnTriggerEnter(Collider coll){
		if (coll.tag == "NpcTrigger") {			
			transform.eulerAngles = new Vector3(transform.eulerAngles.x,-transform.eulerAngles.y,transform.eulerAngles.z);
		}
	}



}
