using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour {

	public Text vibration;

	void Start(){
		
	}

	void OnEnable(){
		CheckVibration ();
	}

	public void OnBackBtn(){
		gameObject.SetActive (false);
	}

	//切换震动状态
	public void OnVibrationBtn(){
		if (PlayerPrefs.GetInt ("vibration", 1) == 1) {
			PlayerPrefs.SetInt ("vibration", 0);
		} else {
			PlayerPrefs.SetInt ("vibration", 1);
		}
		CheckVibration ();
	}
		

	//隐私政策
	public void OnPravicyBtn(){
		Application.OpenURL ("http://www.soulgame.com/policy.html");
	}

	void CheckVibration(){
		bool vibrationState = PlayerPrefs.GetInt ("vibration", 1) == 1;
		if (vibrationState) {
			vibration.text = "vibration : on";
		} else {
			vibration.text = "vibration : false";
		}
	}



}
