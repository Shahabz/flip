using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour {

	void OnEnable(){
		StartCoroutine (FadeText ());
	}
		
	//文字透明度渐变
	IEnumerator FadeText(){
		Text text = GetComponent<Text> ();
		while (true) {			
			text.DOFade (1, 1).OnComplete (()=>{
				text.DOFade (0.65f, 1);
			});
			yield return new WaitForSeconds(2);
		}
	}
}
