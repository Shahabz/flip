using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Shop : MonoBehaviour {
	//hats[hatindex]为当前选择的帽子
	[SerializeField]
	GameObject[] hats;
	int hatIndex;

	[SerializeField]
	Animator animator;
	//统计旋转到哪一个帽子
	float rotateY;
	[SerializeField]
	Material locking;
	Material[] hatMats;
	[SerializeField]
	SkinnedMeshRenderer[] skinRender;

	[SerializeField]
	GameObject buyBtn;
	[SerializeField]
	GameObject chooseBtn;

	//存储每一个帽子的解锁状态及价格
	bool[] lockState;
	int[] price;

	// Use this for initialization
	void Start () {
		rotateY = 13.3f;
		hatMats = new Material[skinRender.Length];
		for (int i = 0; i < hatMats.Length; i++) {
			hatMats [i] = skinRender [i].material;
		}
		InitHat ();
		initHatPrice ();

		hatIndex = PlayerPrefs.GetInt ("HatIndex", 0);
		rotateY += hatIndex * 60;
		UpdateBuyBtn (hatIndex);
		transform.eulerAngles += new Vector3 (0, hatIndex * 60, 0);

		ReadHatActive ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			RotateCy (false);
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			RotateCy (true);
		}
	}


	void RotateCy(bool dir){		
		float yAngle = transform.eulerAngles.y;
		if ((rotateY >= 73f && dir == false) || (rotateY <= 14f + ((hats.Length - 2) * 60) && dir == true)) {
			if (dir) {
				if (hatIndex <= 12 && hatIndex >= 3) {
					hats [hatIndex - 3].SetActive (false);
				}
				if (hatIndex >= 1 && hatIndex <= 10) {
					hats [hatIndex + 3].SetActive (true);
				}
			} else {
				if (hatIndex <= 12 && hatIndex >= 3) {
					hats [hatIndex - 3].SetActive (true);
				}
				if (hatIndex >= 1 && hatIndex <= 10) {
					hats [hatIndex + 3].SetActive (false);
				}
			}

			rotateY += (dir ? 1 : -1) * 60;
			hats [hatIndex].GetComponent<Animator> ().enabled = false;
			hatIndex += (dir ? 1 : -1);
			transform.DORotate (new Vector3 (0, rotateY, 0), 0.3f, RotateMode.Fast).OnComplete (()=>{
				Animator hatAnim = hats [hatIndex].GetComponent<Animator> ();
				hatAnim.enabled = true;
				hatAnim.SetTrigger("ChangeHat");
				animator.SetTrigger("ChangeHat");
			});
			//Debug.Log (hats [hatIndex].name);
			UpdateBuyBtn(hatIndex);
			PlayerPrefs.SetInt ("HatIndex", hatIndex);

			SaveHatActive ();
		}
	}

	//刷新帽子解锁状态
	void InitHat(){
		lockState = new bool[hats.Length];
		for(int i=1;i<hats.Length;i++){			
			if (PlayerPrefs.GetInt (hats [i].name, 0) == 0) {
				skinRender [i].material = locking;
				hats [i].transform.Find ("lock").gameObject.SetActive (true);
				lockState [i] = false;
			} else {
				lockState [i] = true;
			}
		}
	}
		

	//刷新购买按钮
	void UpdateBuyBtn(int index){		
		if (lockState [index]||index==0) {
			chooseBtn.SetActive (true);
			buyBtn.SetActive (false);
		} else {
			chooseBtn.SetActive (false);
			buyBtn.transform.Find ("price").GetComponent<Text> ().text = price [index].ToString();
			buyBtn.SetActive (true);
		}
	}

	public void OnBuyBtn(){
		
	}

	public void OnChooseBtn(){
		
	}

	void initHatPrice(){
		price = new int[] {0,50,100,100,100,100,100,100,250,250,250,500,500,1000};
	}

	void SaveHatActive(){
		for (int i = 0; i < lockState.Length; i++) {
			if (hats [i].activeSelf)
				PlayerPrefs.SetInt ("ActiveHat" + i, 1);
			else {
				PlayerPrefs.SetInt ("ActiveHat" + i, 0);
			}
		}
		PlayerPrefs.SetInt ("SaveHatActive", 1);
	}

	void ReadHatActive(){
		if (PlayerPrefs.GetInt ("SaveHatActive", 0) == 1) {
			for (int i = 0; i < lockState.Length; i++) {
				if (PlayerPrefs.GetInt ("ActiveHat" + i, 0) == 1) {
					hats [i].SetActive (true);
				} else {
					hats [i].SetActive (false);
				}
			}
			Animator hatAnim = hats [hatIndex].GetComponent<Animator> ();
			hatAnim.enabled = true;
			hatAnim.SetTrigger("ChangeHat");
			animator.SetTrigger("ChangeHat");
		}
	}

}
