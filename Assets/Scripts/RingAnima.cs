using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RingAnima : MonoBehaviour {
	Material mat;
	Color initColor;
	// Use this for initialization
	void Start () {
		mat = GetComponent<MeshRenderer> ().material;
		initColor = mat.color;
		StartCoroutine (RingSizeAdd ());
	}
	
	IEnumerator RingSizeAdd(){
		while (true) {
			mat.DOFade (0, "_Color", 1.5f).OnComplete(()=>{
				mat.color = initColor;
			});
			transform.DOScale (1.4f, 1.5f).OnComplete (()=>{
				transform.localScale = Vector3.one;
			});
			yield return new WaitForSeconds(2f);
		}
	}
}
