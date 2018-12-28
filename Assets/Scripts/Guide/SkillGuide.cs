using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SkillGuide : MonoBehaviour {
	public GameObject motion;
	public GameObject levelup;
	public Transform bg;

	public GameObject[] point;

	static SkillGuide instance;
	public static SkillGuide Instance{
		get { return instance; }
	}   
	private void Awake()
	{
		instance = this;
	}

	void ShowLevelup(){
		GameObject levelupGo = Instantiate(levelup,levelup.transform.position,levelup.transform.rotation,bg);
		point [1].SetActive (true);
		levelupGo.transform.Find ("Upgrade").GetComponent<Button> ().interactable = true;
		levelupGo.transform.Find("Upgrade").GetComponent<Button> ().onClick.AddListener (() => {
			Destroy (levelupGo);
			point [1].SetActive (false);
			bg.gameObject.SetActive (false);
			PlayerPrefs.SetInt ("SkillFinish", 1);
		});
	}

	public void StartGuide () {
		bg.gameObject.SetActive (true);
		GameObject motionGO = Instantiate (motion,motion.transform.position,motion.transform.rotation,bg);
		motionGO.SetActive (true);
		if (motionGO.activeSelf) {			
			point [0].SetActive (true);
			motionGO.GetComponent<Button> ().onClick.AddListener (() => {
				Destroy (motionGO);
				point [0].SetActive (false);
				Invoke ("ShowLevelup", 0.5f);
			});
		}
	}

	void Update(){

	}
}
