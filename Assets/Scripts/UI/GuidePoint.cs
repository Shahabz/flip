using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GuidePoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (point ());
	}
	
	IEnumerator point(){
		while (true) {
			if (transform.name.EndsWith ("up")) {
				transform.DOPunchPosition (Vector3.up * 5, 1, 5, 1, false);
			}
			if (transform.name.EndsWith ("down")) {
				transform.DOPunchPosition (Vector3.down * 5, 1, 5, 1, false);
			}
			if (transform.name.EndsWith ("left")) {
				transform.DOPunchPosition (Vector3.left * 5, 1, 5, 1, false);
			}
			if (transform.name.EndsWith ("right")) {
				transform.DOPunchPosition (Vector3.right * 5,1, 5, 1, false);
			}
			yield return new WaitForSeconds(1);
		}
	}
}
