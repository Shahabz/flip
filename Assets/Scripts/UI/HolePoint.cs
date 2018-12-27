using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HolePoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		Quaternion targetQuat = Quaternion.FromToRotation(Vector3.back, new Vector3((Camera.main.transform.position - transform.position).x, 0, (Camera.main.transform.position - transform.position).z));
		transform.rotation = targetQuat;
		transform.eulerAngles += new Vector3 (0, 0, -90);
		StartCoroutine (point ());
		
	}

	IEnumerator point(){
		while (true) {
			transform.DOPunchPosition (-Vector3.up/2, 1, 3, 1, false);
			yield return new WaitForSeconds(1);
		}
	}
}
