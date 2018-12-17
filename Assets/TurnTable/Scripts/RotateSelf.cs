using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class RotateSelf : MonoBehaviour {
 
    void OnEnable () {
		StartCoroutine (Rotate ());
	}
	
	IEnumerator Rotate(){
		yield return new WaitForSeconds (0.5f);
		while (true) {
			transform.DORotate (transform.eulerAngles - new Vector3 (0,0,1720), 4f, RotateMode.FastBeyond360);
			yield return new WaitForSeconds (5);
		}
	}

}
