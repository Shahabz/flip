using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Together;

public class DoubleSettle : MonoBehaviour {

	public Text goldText;
	public Transform player;
	int passGold;
	void OnEnable(){
		passGold = PlayerPrefs.GetInt ("LevelPassGold", 0);
		goldText.text = passGold.ToString ();
		transform.SetAsLastSibling ();
	}

	public void OnDoubleBtn(){
		if (TGSDK.CouldShowAd (TGSDKManager.doubleID)) {
			TGSDK.ShowAd (TGSDKManager.doubleID);
			TGSDK.AdCloseCallback = (string obj) => {
				FlyGold.Instance.GenerateGoldNoColl (20, player.position);
				Gold.Instance.GetGold(passGold);
				MoneyManager.Instance.UpdateGold();
				Time.timeScale = 1;
			};
		} else {
			TipPop.GenerateTip ("no ads", 1f);
			OnExitBtn ();
		}
	}

	public void OnExitBtn(){
		Time.timeScale = 1;
		gameObject.SetActive (false);

	}

}
