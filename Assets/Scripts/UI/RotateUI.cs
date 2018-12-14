using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotateUI : MonoBehaviour {
	void Start(){
		StartCoroutine (RotateText ());
	}

	IEnumerator RotateText(){
		
		while (transform) {
			transform.DOScale (0.6f, 0.4f);
			transform.DORotate (new Vector3 (0, 0, 7), 0.5f, RotateMode.Fast).OnComplete(()=>{				
				transform.DOScale (1f, 0.4f);
				transform.DORotate (new Vector3 (0, 0, 0), 0.5f, RotateMode.Fast).OnComplete(()=>{
					transform.DOScale (0.6f, 0.4f);
					transform.DORotate (new Vector3 (0, 0, -7), 0.5f, RotateMode.Fast).OnComplete(()=>{
						transform.DOScale (1f, 0.4f);
						transform.DORotate (new Vector3 (0, 0, 0), 0.5f, RotateMode.Fast).OnComplete(()=>{

						});
					});
				});
			});
			yield return new WaitForSeconds (2);
		}
	}
}
