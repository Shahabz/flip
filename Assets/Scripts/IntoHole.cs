using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntoHole : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<SkinnedMeshRenderer>().material.renderQueue = 2001;
        print(GetComponent<SkinnedMeshRenderer>().material.renderQueue);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
