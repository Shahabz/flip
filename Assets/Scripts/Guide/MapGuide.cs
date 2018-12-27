using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapGuide : MonoBehaviour {
	public GameObject home;
	public GameObject menuMove;
	public GameObject map;
	public Transform bg;

	public GameObject[] point;

	static MapGuide instance;
	public static MapGuide Instance{
		get { return instance; }
	}   
	private void Awake()
	{
		instance = this;
	}

	void Start(){
		if (PlayerPrefs.GetInt ("MapHomeFinish", 0) == 1) {	
			PlayerPrefs.SetInt ("MapHomeFinish", 2);
			bg.gameObject.SetActive (true);
			point [1].SetActive (true);
			GameObject moveGO = Instantiate (menuMove, bg);
			moveGO.GetComponent<Button> ().onClick.AddListener (() => {
				Destroy (moveGO);
				point [1].SetActive (false);
				Invoke ("ShowMap", 0.5f);
			});
		}
	}

	void ShowMap(){
		GameObject mapGo = Instantiate(map,map.transform.position,map.transform.rotation,bg);
		point [2].SetActive (true);
		mapGo.GetComponent<Button> ().onClick.AddListener (() => {
			Destroy (mapGo);
			point [2].SetActive (false);
			bg.gameObject.SetActive (false);
		});
	}

	public void StartGuide () {
		bg.gameObject.SetActive (true);
		GameObject homeGO = Instantiate (home,bg);
		homeGO.SetActive (true);
		if (homeGO.activeSelf) {			
			PlayerPrefs.SetInt ("MapHomeFinish", 1);
			point [0].SetActive (true);
		}
	}

	void Update(){
		
	}
}
