using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadingUI : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		StartCoroutine (Loading ());
	}
	
	IEnumerator Loading(){
		while (true) {
			transform.DORotate (new Vector3 (0, 0, -360), 1, RotateMode.FastBeyond360);
			yield return new WaitForSeconds (1);
		}
	}
}
