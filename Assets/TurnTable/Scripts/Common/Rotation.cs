using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rotation : MonoBehaviour {

	public delegate void RotationFinishDe();
	public event RotationFinishDe RotationFinish;

	Vector3 rotateAngle = new Vector3(0,0,2000);
	public float time = 4;

	bool stopRotate = true;

	void OnEnable(){
		rotateAngle = new Vector3 (0, 0, Random.Range (1900, 2261));
	}

	public void RotateThis(){
		transform.DORotate (transform.rotation.eulerAngles + rotateAngle, time, RotateMode.FastBeyond360).OnComplete(()=>{
			if (RotationFinish != null) {
				RotationFinish ();
			}
		});
	}

	public void RotateLittle(){
		transform.DORotate (transform.rotation.eulerAngles + new Vector3(0,0,20), time, RotateMode.FastBeyond360);
	}

}
