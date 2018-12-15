using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pointer : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
        StartCoroutine(PointerPunch());
	}
	
    IEnumerator PointerPunch(){
        while(true){
            if (transform.name == "pointer1")
            {
                transform.DOPunchPosition(new Vector3(0, 10, 0), 1, 5, 1, false);
            }
            if (transform.name == "pointer2")
            {
                transform.DOPunchPosition(new Vector3(10, 0, 0), 1, 5, 1, false);
            }
            yield return new WaitForSeconds(1);
        }
    }
}
