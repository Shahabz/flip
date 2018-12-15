using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHat : MonoBehaviour {
	public string[] hatsName;
	public GameObject[] hatsGo;
	Dictionary<string,GameObject> strToHatGO;

	void Start(){
		strToHatGO = new Dictionary<string, GameObject> ();
		InitHatDic ();
		ChooseHat ();
	}

	void InitHatDic(){
		for (int i = 0; i < hatsName.Length; i++) {
			strToHatGO.Add (hatsName [i], hatsGo [i]);
		}
	}

	void ChooseHat(){
		string hatName = PlayerPrefs.GetString ("CurrentHat", "trucker");
		string lastHatName =  PlayerPrefs.GetString ("LastHat", "trucker");
		if (lastHatName != hatName) {
			strToHatGO [lastHatName].SetActive (false);
			strToHatGO [hatName].SetActive (true);
		}
	}
}
