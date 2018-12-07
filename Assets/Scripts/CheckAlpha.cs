using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAlpha : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
		StartCoroutine (Perspective.Instance.CheckObstacle (gameObject));
	}
	

}
