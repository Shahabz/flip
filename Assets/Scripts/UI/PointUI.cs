using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PointUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetInt ("OtherGuide", 0) == 0) {
			StartCoroutine (point ());
		} else {
			transform.parent.gameObject.SetActive (false);
		}
	}
	
	IEnumerator point(){
		while (true) {
			transform.DOPunchPosition (Vector3.left*3, 2, 10, 1, false);
			yield return new WaitForSeconds(2);
		}
	}

	public void OnGuideClick(){
		transform.parent.gameObject.SetActive (false);
		PlayerPrefs.SetInt ("OtherGuide", 1);
	}
}
