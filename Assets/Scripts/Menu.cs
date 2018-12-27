using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Together;

public class Menu : MonoBehaviour {
	[SerializeField]
	Transform moveBtn;
	[SerializeField]
	GameObject shop;
	[SerializeField]
	GameObject setting;
	[SerializeField]
	public GameObject turnTable;
	[SerializeField]
	GameObject mission;
	public GameObject turnBtn;
	bool moveFinish = true;

	static Menu instance;
	public static Menu Instance
	{
		get { return instance; }
	}
	private void Awake()
	{
		instance = this;
	}

	void Start(){
		AutoPopDaily ();
	}

	public void OnMoveBtn(){
		if (moveFinish) {
			moveFinish = false;
			if (moveBtn.eulerAngles.z == 0) {
				transform.DOMoveX (transform.position.x + Screen.width*0.07f, 0.5f, false).OnComplete (() => {
					moveBtn.eulerAngles = new Vector3 (0, 0, 180);
					moveFinish = true;
				});
			} else if (moveBtn.eulerAngles.z == 180) {
				transform.DOMoveX (transform.position.x - Screen.width*0.07f, 0.5f, false).OnComplete (() => {
					moveBtn.eulerAngles = new Vector3 (0, 0, 0);
					moveFinish = true;
				});
			}
		}
	}

	public void OnClothBtn(){
		MySceneManager.LoadScene ("Shop");
	}

	public void OnShopBtn(){
		shop.SetActive (true);
		turnBtn.SetActive (false);
	}

	public void OnSettingBtn(){
		setting.SetActive (true);
	}

	public void OnMissionBtn(){
		PlayerPrefs.SetInt ("OnMissionBtn", 1);
		mission.SetActive (true);
		turnBtn.SetActive (false);
	}

	//先看广告再转转盘
	public void OnTurnBtn(){
		if (PlayerPrefs.GetInt ("TurnHomeFinish", 0) == 1) {			
			return;
		}
		if (TGSDK.CouldShowAd (TGSDKManager.turnID)) {
			TGSDK.ShowAd (TGSDKManager.turnID);
			TGSDK.AdCloseCallback = (string obj) => {
				turnTable.SetActive (true);
			};
		} else {
			TipPop.GenerateTip ("no ads", 1);
		}
	}

	void AutoPopDaily(){
		if (PlayerPrefs.GetInt ("AutoDaily", 1) == 1) {
			OnMissionBtn ();
			PlayerPrefs.SetInt ("AutoDaily", 0);
		}
	}
}
