using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RingLight : MonoBehaviour {
	Light ringLight;
	[SerializeField]
	float time;
	// Use this for initialization
	void Start () {
		ringLight = GetComponent<Light> ();
		StartCoroutine (LightChange ());
	}
	
	IEnumerator LightChange(){
		while(true){
			ringLight.DOIntensity(0.5f,time).OnComplete(()=>{
				ringLight.DOIntensity(0,time);
			});
			yield return new WaitForSeconds(time*2);
		}
	}
}
