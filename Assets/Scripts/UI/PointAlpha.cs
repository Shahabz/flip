using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PointAlpha : MonoBehaviour {
	Image image;
	Color initColor;
	Vector3 initPos;
	// Use this for initialization
	void Awake(){	
		image = GetComponent<Image> ();	
		initPos = Vector3.zero;
		initColor = image.color;
	}
	void OnEnable () {			
		transform.localPosition = initPos;
		image.color = initColor;
		StartCoroutine ("LeftAlpha");
	}
	
	IEnumerator LeftAlpha(){
		
		while (true) {
			transform.parent.DOScale (0.85f, 0.2f).OnComplete (()=>{
				transform.parent.DOScale (1f, 0.1f);
				transform.DOLocalMoveX (50, 1, false).OnComplete(()=>{
					transform.localPosition = initPos;
				});
				image.DOFade (0, 1).OnComplete (()=>{
					image.color = initColor;
				});
			});

			yield return new WaitForSeconds (1.3f);
		}
	}
}
