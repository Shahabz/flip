using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
	[SerializeField]
	Transform moveBtn;
	[SerializeField]
	GameObject shop;
	[SerializeField]
	GameObject setting;
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

	public void OnMoveBtn(){
		if (moveFinish) {
			moveFinish = false;
			if (moveBtn.eulerAngles.z == 0) {
				transform.DOMoveX (transform.position.x + 95, 0.5f, false).OnComplete (() => {
					moveBtn.eulerAngles = new Vector3 (0, 0, 180);
					moveFinish = true;
				});
			} else if (moveBtn.eulerAngles.z == 180) {
				transform.DOMoveX (transform.position.x - 95, 0.5f, false).OnComplete (() => {
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
	}

	public void OnSettingBtn(){
		setting.SetActive (true);
	}
}
