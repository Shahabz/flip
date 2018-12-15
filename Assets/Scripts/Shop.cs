using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

	//切换帽子选择，true为往右看，false往左看
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
			transform.DOScale (0.6f,0.15f).OnComplete (()=>{
				transform.DOScale (1f,0.15f);
			});
			UpdateBuyBtn(hatIndex);
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

	//购买商品后更换材质，隐藏锁，保存购买信息，装备皮肤
	public void OnBuyBtn(){
		if (Diamond.Instance.UseDiamond (price [hatIndex])) {
			PlayerPrefs.SetInt (hats [hatIndex].name, 1);
			skinRender [hatIndex].material = hatMats[hatIndex];
			hats [hatIndex].transform.Find ("lock").gameObject.SetActive (false);
			OnChooseBtn ();
		}
	}

	//选择皮肤，下次打开商城自动切换到该皮肤
	public void OnChooseBtn(){	
		PlayerPrefs.SetString ("CurrentHat", hats [hatIndex].name);
		PlayerPrefs.SetInt ("HatIndex", hatIndex);
		SaveHatActive ();
	}

	void initHatPrice(){
		price = new int[] {0,50,100,100,100,100,100,100,250,250,250,500,500,1000};
	}

	//保存当前帽子的显示和隐藏状态
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

	//读取当前帽子的显示和隐藏状态
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

	public void GoHome(){
		MySceneManager.LoadScene ("SampleScene");
	}

}
