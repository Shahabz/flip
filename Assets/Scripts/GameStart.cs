using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameStart : MonoBehaviour {
	[SerializeField]
	Transform firstHole;
	[SerializeField]
	CameraRotate cameraRotate;
	[SerializeField]
	GameObject level;

	//点击开始按钮则隐藏开始按钮、黑洞出现、调整重力，取消脚和身体的碰撞，状态设置为跳跃，开始蓄力动画，开始旋转
	//到达给定角度则取消角色位置限制，开始跳跃动画，增加跳跃力，翻转360度，1秒后恢复脚和身体碰撞，摄像机开始跟随，角色可以开始update
	public void OnStartClick(){
		gameObject.SetActive (false);
		firstHole.DOLocalMoveY (3, 1, false);
		PlayerController.Instance.GameStart ();
		PlayerController.Instance.Starting = true;
		cameraRotate.enabled = true;
	}

}
