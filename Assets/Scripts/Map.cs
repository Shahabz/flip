using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Map : MonoBehaviour {
	public GameObject[] environment;
	public GameObject map;
	public CameraRotate cameraRotate;
	[SerializeField]
	Image writeImage;
	// Use this for initialization

	public void EnterMap(){
		map.SetActive (true);
	}

	public void ExitMap(){
		map.SetActive (false);
	}

	public void ChangeToMap(int index){
		ExitMap ();
		writeImage.gameObject.SetActive (true);
		writeImage.color = new Color (200/255f, 200/255f, 200f/255, 255/255f);
		writeImage.DOFade (0, 0.5f).OnComplete (()=>{
			writeImage.gameObject.SetActive(false);
		});

		int count = environment.Length;
		for (int i = 0; i < count; i++) {
			environment [i].SetActive (i == index);
		}
	}
}
