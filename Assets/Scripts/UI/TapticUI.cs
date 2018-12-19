using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapticUI : MonoBehaviour {
	void OnEnable(){
		GetComponent<Button> ().onClick.AddListener (OnButtonClick);
	}

	public void OnButtonClick(){
		if (PlayerPrefs.GetInt ("vibration", 1)==1) {
			MultiHaptic.HapticLight ();
		}
	}
}
