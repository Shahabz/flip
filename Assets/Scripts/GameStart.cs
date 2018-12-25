using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GameStart : MonoBehaviour {
	[SerializeField]
	Transform firstHole;
	[SerializeField]
	CameraRotate cameraRotate;
	[SerializeField]
	GameObject level;
	[SerializeField]
	Image writeImage;
	[SerializeField]
	MeshRenderer hole;
	[SerializeField]
	MeshRenderer mask;
	GameObject[] gameUIs;
	GameObject[] moneyUIs;

	void Start(){
		CheckChangeLevel ();
		gameUIs = GameObject.FindGameObjectsWithTag ("StartUI");
		moneyUIs = GameObject.FindGameObjectsWithTag ("money");
	}

	//点击开始按钮则隐藏开始按钮、黑洞出现、调整重力，取消脚和身体的碰撞，状态设置为跳跃，开始蓄力动画，开始旋转
	//到达给定角度则取消角色位置限制，开始跳跃动画，增加跳跃力，翻转360度，1秒后恢复脚和身体碰撞，摄像机开始跟随，角色可以开始update
	public void OnStartClick(){
		HideStartUI (true);
		HideMoneyUI (true);
		firstHole.DOLocalMoveY (3, 1, false);
		PlayerController.Instance.GameStart ();
		PlayerController.Instance.Starting = true;
		cameraRotate.enabled = true;
	}

	void HideStartUI(bool hide){		
		foreach (GameObject go in gameUIs) {
			go.SetActive (!hide);
		}
	}
		
	void HideMoneyUI(bool hide){		
		foreach (GameObject go in moneyUIs) {
			go.SetActive (!hide);
		}
	}

	public void ChangeLevel(){
		PlayerPrefs.SetInt ("changeLevel", 1);
		PlayerController.Instance.GameOver ();	
	}

	int change = 0;
	void CheckChangeLevel(){
		change = PlayerPrefs.GetInt ("changeLevel", 0);
		if (change == 1) {
			hole.enabled = false;
			mask.enabled = false;
			writeImage.gameObject.SetActive (true);
			writeImage.DOColor (new Color(0.95f,0.95f,0.95f), 0.5f).OnComplete (()=>{
				writeImage.DOColor (Color.clear, 1.5f).OnComplete (()=>{
					Destroy(writeImage.gameObject);
				});
			});

			cameraRotate.enabled = true;
			gameObject.SetActive (false);
			PlayerController.Instance.ChangeLevel ();
			PlayerController.Instance.Starting = true;
			PlayerPrefs.SetInt ("changeLevel", 0);
		}
	}


}
