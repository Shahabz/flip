using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ScaleUI2 : MonoBehaviour {

	void OnEnable(){
		GetComponent<Button> ().onClick.AddListener (OnButtonClick);
	}

	public void OnButtonClick(){
		transform.parent.DOScale (1.1f, 0.15f).OnComplete (()=>{
			transform.parent.DOScale (1f, 0.15f);
		});
	}
}
