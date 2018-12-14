using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Menu : MonoBehaviour {
	[SerializeField]
	Transform moveBtn;
	bool moveFinish = true;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
}
