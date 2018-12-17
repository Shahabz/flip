using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Together;
using DG.Tweening;

public class Daily : MonoBehaviour {
	public Transform content;
	public GameObject mission;
	public GameObject freeMission;
	[SerializeField]
	Transform transEx;
	void OnEnable(){		
		missionGO = new List<Transform>();
		missionProgress = new Dictionary<string, string> ();

		for (int i = 0; i < 7; i++) {
			missionGO.Add (transEx);
		}

		SetMission ();
		SetMissionProgress ();
		UpdateMission ();
		CheckProgress ();
		content.localPosition = new Vector3 (0, 2, 0);

		if (!missionChange) {
			OnBackBtn ();
		}
	}
	
	public void OnBackBtn(){
		gameObject.SetActive (false);
		if (PlayerPrefs.GetInt ("NewDayTurn", 0) == 1) {	
			Menu.Instance.turnBtn.SetActive (true);
		}
		for (int i = 0; i < missionGO.Count; i++) {	
			//Debug.Log (missionGO [i].name);	
			if (missionGO [i]) {	
				Destroy (missionGO [i].gameObject);
			}
		}
	}

	public void OnFreeMissionBtn(){
		if (TGSDK.CouldShowAd (TGSDKManager.resetDailyID)) {
			TGSDK.ShowAd (TGSDKManager.resetDailyID);
			TGSDK.AdCloseCallback = (string obj) => {
				PlayerPrefs.SetInt ("freeMission", 1);
				UpdateMission ();
			};
		} else {
			TipPop.GenerateTip ("no ads", 1f);
		}
	}

	bool missionChange = false;
	string[] missionChangeStr = new string[4];
	void UpdateMission(){	
		missionChange = false;

		for (int i = 1; i < 4; i++) {
			
			if (PlayerPrefs.GetInt ("Mission" + i, 1) == 1) {				
				Transform go = Instantiate (mission, content).transform;
				missionGO[i-1]= go;
				string Mission_name;
				if (PlayerPrefs.GetString ("Mission_name" + i, "null") == "null") {
					Mission_name = missionName [Random.Range (0, missionName.Count)];
					missionName.Remove (Mission_name);
					PlayerPrefs.SetString ("Mission_name" + i, Mission_name);
				} else {
					Mission_name = PlayerPrefs.GetString ("Mission_name" + i, "null");
				}

				go.Find ("content").GetComponent<Text> ().text = Mission_name;
				if (missionChangeStr [i] != (PlayerPrefs.GetInt (missionProgress [Mission_name], 0) + "/" + 10 * i)) {					
					missionChange = true;
				}
				go.Find ("progress").GetComponent<Text> ().text = PlayerPrefs.GetInt (missionProgress[Mission_name], 0) + "/" + 10 * i;
				missionChangeStr [i] = PlayerPrefs.GetInt (missionProgress [Mission_name], 0) + "/" + 10 * i;
				go.Find ("diamond").GetComponent<Text> ().text = (10 * i).ToString ();				
			}
		}
		if (PlayerPrefs.GetInt ("freeMission", 0) == 0) {
			Transform freeGo = Instantiate (freeMission, content).transform;
			freeGo.GetComponent<Button> ().onClick.AddListener (OnFreeMissionBtn);

			missionGO[3] =freeGo;
		} else if (PlayerPrefs.GetInt ("freeMission", 0) == 1){
			Transform go = Instantiate (mission, content).transform;

			missionGO[3] =go;
			string Mission_name;
			if (PlayerPrefs.GetString ("Mission_name4", "null") == "null") {
				Mission_name = missionName [Random.Range (0, missionName.Count)];
				missionName.Remove (Mission_name);
				PlayerPrefs.SetString ("Mission_name4", Mission_name);
			} else {
				Mission_name = PlayerPrefs.GetString ("Mission_name4", "null");
			}

			go.Find ("content").GetComponent<Text> ().text = Mission_name;
			if (missionChangeStr [3] != (PlayerPrefs.GetInt (missionProgress[Mission_name], 0) + "/" + 30).ToString()) {
				missionChange = true;
			}
			go.Find ("progress").GetComponent<Text> ().text = PlayerPrefs.GetInt (missionProgress[Mission_name], 0) + "/" + 30;
			missionChangeStr [3] = PlayerPrefs.GetInt (missionProgress [Mission_name], 0) + "/" + 30;
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

	Dictionary<string,string> missionProgress;
	void SetMissionProgress(){
		missionProgress.Add ("Land on the roof", "Roof");
		missionProgress.Add ("Hit by the fist", "GloveHit");
		missionProgress.Add ("hit by the car", "CarHit");
		missionProgress.Add ("Land on the road", "Road");
		missionProgress.Add ("Lond on the car", "CarUp");
			
	}

	List<Transform> missionGO;
	void CheckProgress(){
		for (int i = 1; i < 4; i++) {
			if (PlayerPrefs.GetInt ("Mission" + i, 1) == 1) {
				if (PlayerPrefs.GetInt (missionProgress [PlayerPrefs.GetString ("Mission_name" + i, "null")], 0) >= i * 10) {				
					Diamond.Instance.GetDiamond (i * 10);
					PlayerPrefs.SetInt ("Mission" + i, 0);
					int index = i-1;
					missionGO [index].DOScale (0, 0.5f).OnComplete (() => {
						
						Destroy (missionGO [index].gameObject);

					});

				}
			}
		}
		if (PlayerPrefs.GetInt ("freeMission", 0) == 1) {
			if (PlayerPrefs.GetInt (missionProgress [PlayerPrefs.GetString ("Mission_name4", "null")], 0) >= 30) {
				Diamond.Instance.GetDiamond (50);
				PlayerPrefs.SetInt ("freeMission", 2);

				missionGO[3].DOScale (0, 0.5f).OnComplete(()=>{

					Destroy(missionGO[3].gameObject);
				});
			
			}
		}

	}
}
