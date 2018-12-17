using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aresome : MonoBehaviour {
	public bool dir;
	// Use this for initialization
	void Start () {
		StartCoroutine (rotate ());
	}
	
	IEnumerator rotate(){
		while (true) {
			transform.Rotate (new Vector3 (0, 0, dir ? 0.5f : -0.5f));
			yield return null;
		}
	}
}
