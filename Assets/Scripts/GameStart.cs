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
	[SerializeField]
	Animator playerAnima;

	// Use this for initialization
	void Start () {
		
	}
	
	public void OnStartClick(){
		gameObject.SetActive (false);
		firstHole.DOLocalMoveY (3, 1, false);
		PlayerController.Instance.GameStart ();
		PlayerController.Instance.Starting = true;
		cameraRotate.enabled = true;
		Invoke ("LevelDaily", 3f);
		playerAnima.avatar = null;
	}

	void LevelDaily(){
		level.SetActive (true);
	}
}
