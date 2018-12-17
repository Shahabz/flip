using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Together;

public class Daily : MonoBehaviour {
	public Transform content;
	public GameObject mission;
	public GameObject freeMission;


	void OnEnable(){
		SetMission ();
		UpdateMission ();
	}
	
	public void OnBackBtn(){
		gameObject.SetActive (false);
		if (PlayerPrefs.GetInt ("NewDayTurn", 0) == 1) {	
			Menu.Instance.turnBtn.SetActive (true);
		}
	}

	public void OnFreeMissionBtn(){
		if (TGSDK.CouldShowAd (TGSDKManager.resetDailyID)) {
			TGSDK.ShowAd (TGSDKManager.resetDailyID);
			TGSDK.AdCloseCallback = (string obj) => {
				PlayerPrefs.SetInt ("freeMission", 1);
				UpdateMission();
			};
		}
	}

	void UpdateMission(){		
		for (int i = 1; i < 4; i++) {
			if (PlayerPrefs.GetInt ("Mission" + i, 1) == 1) {				
				Transform go = Instantiate (mission, content).transform;
				string Mission_name;
				if (PlayerPrefs.GetString ("Mission_name" + i, "null") == "null") {
					Mission_name = missionName [Random.Range (0, missionName.Count)];
					missionName.Remove (Mission_name);
					PlayerPrefs.SetString ("Mission_name" + i, Mission_name);
				} else {
					Mission_name = PlayerPrefs.GetString ("Mission_name" + i, "null");
				}

				go.Find ("content").GetComponent<Text> ().text = Mission_name;
				go.Find ("progress").GetComponent<Text> ().text = PlayerPrefs.GetInt ("Mission_number" + i, 0) + "/" + 10 * i;
				go.Find ("diamond").GetComponent<Text> ().text = (10 * i).ToString ();				
			}
		}

		if (PlayerPrefs.GetInt ("freeMission", 0) == 0) {
			Transform freeGo = Instantiate (freeMission, content).transform;
			freeGo.GetComponent<Button> ().onClick.AddListener (OnFreeMissionBtn);
		} else if (PlayerPrefs.GetInt ("freeMission", 0) == 1){
			Transform go = Instantiate (mission, content).transform;
			string Mission_name;
			if (PlayerPrefs.GetString ("Mission_name4", "null") == "null") {
				Mission_name = missionName [Random.Range (0, missionName.Count)];
				missionName.Remove (Mission_name);
				PlayerPrefs.SetString ("Mission_name4", Mission_name);
			} else {
				Mission_name = PlayerPrefs.GetString ("Mission_name4", "null");
			}

			go.Find ("content").GetComponent<Text> ().text = Mission_name;
			go.Find ("progress").GetComponent<Text> ().text = PlayerPrefs.GetInt ("Mission_number4", 0) + "/" + 30;
			go.Find ("diamond").GetComponent<Text> ().text = "50";
		}
	}


	List<string> missionName = new List<string>();
	void SetMission(){
		missionName.Add("Land on the roof");
		missionName.Add("Hit by the fist");
		missionName.Add("hit by the car");
		missionName.Add("Land on the road");
		missionName.Add("Lond on the car");
	}
}
