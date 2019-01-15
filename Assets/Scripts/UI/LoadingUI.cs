using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LoadingUI : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		StartCoroutine (Loading ());
		StartCoroutine (WaitLoading ());
	}
	
	IEnumerator Loading(){
		while (true) {
			transform.DORotate (new Vector3 (0, 0, -360), 1, RotateMode.FastBeyond360);
			yield return new WaitForSeconds (1);
		}
	}

	IEnumerator WaitLoading(){
		AsyncOperation op = SceneManager.LoadSceneAsync ("SampleScene");
		op.allowSceneActivation = false;
		while (true) {
			if (op.progress == 0.9f) {
				op.allowSceneActivation = true;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
}
