using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Map : MonoBehaviour {	
	GameObject[] environments;
	public Enviroment environment;
	public GameObject map;
	public CameraRotate cameraRotate;
	public GameObject[] curLock;
	public Text price;
	public GameObject purchasePop;
	public GameObject[] pointers;
	public RectTransform content;
	public Radar radar;
	[SerializeField]
	Image writeImage;
	// Use this for initialization

	void Start(){		
		environments = environment.scenesMenu;
		int curMapIndex = PlayerPrefs.GetInt ("CurMap", 0);
		ChangeToMap (curMapIndex);
		InitPlayerAndRadar (curMapIndex);
	}

	void InitPlayerAndRadar(int index){
		PlayerController.Instance.city = environments [index];
		radar.city = environments [index].transform;
	}

	public void EnterMap(){
		map.SetActive (true);
		UpdateLock ();
		UpdatePointer ();
		Menu.Instance.OnMoveBtn ();
		Menu.Instance.turnBtn.SetActive (false);
	}

	public void ExitMap(){
		map.SetActive (false);
		if (PlayerPrefs.GetInt ("NewDayTurn", 0) == 1) {	
			Menu.Instance.turnBtn.SetActive (true);
		}
	}

	//进入地图
	public void ChangeToMap(int index){
		ExitMap ();
		writeImage.gameObject.SetActive (true);
		writeImage.color = new Color (200/255f, 200/255f, 200f/255, 255/255f);
		writeImage.DOFade (0, 0.5f).OnComplete (()=>{
			writeImage.gameObject.SetActive(false);
		});

		int count = environments.Length;
		for (int i = 0; i < count; i++) {
			environments [i].SetActive (i == index);
		}
		PlayerPrefs.SetInt ("CurMap", index);
	}

	//刷新地图解锁
	void UpdateLock(){
		for (int i = 1; i < curLock.Length; i++) {
			if (PlayerPrefs.GetInt ("CurLock" + i, 0) == 1) {
				curLock [i].SetActive (false);
			}
		}
	}

	//打开解锁付费界面
	public void UnlockMap(int index){
		if (index == 1) {
			price.text = "-50,000";
			priceInt = 50000;
			mapIndex = 1;
		}if (index == 2) {
			price.text = "-5,000,000";
			priceInt = 5000000;
			mapIndex = 2;
			content.localPosition = new Vector3 (-557, 0, 0);
		}
		purchasePop.SetActive (true);
	}

	//进行解锁
	int mapIndex = 0;
	int priceInt = 0;
	public void PurchaseMap(){
		if (Gold.Instance.UseGold (priceInt)) {
			PlayerPrefs.SetInt ("CurLock" + mapIndex, 1);
			curLock [mapIndex].SetActive (false);
			purchasePop.SetActive (false);
			ChangeToMap (mapIndex);
			MoneyManager.Instance.UpdateGold();
		}
	}

	public void PurchaseClose(){
		purchasePop.SetActive (false);
	}

	//刷新指针显隐,移动地图位置
	void UpdatePointer(){		
		int curMapIndex = PlayerPrefs.GetInt ("CurMap", 0);
		for (int i = 0; i < pointers.Length; i++) {
			pointers [i].SetActive (curMapIndex == i);
		}
		if (curMapIndex == 0 || curMapIndex == 1) {
			content.localPosition = new Vector3 (0, 0, 0);
		} else if (curMapIndex == 2) {
			content.localPosition = new Vector3 (-557, 0, 0);
		}
	}


}
