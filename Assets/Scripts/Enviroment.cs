using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enviroment : MonoBehaviour {
	public GameObject[] scenes;
	[HideInInspector]
	public GameObject[] scenesMenu;

	// Use this for initialization
	void Start () {
		scenesMenu = new GameObject[scenes.Length];
		for(int i=0;i<scenes.Length;i++){
			scenesMenu[i] = Instantiate (scenes [i]);
		}
	}

}
