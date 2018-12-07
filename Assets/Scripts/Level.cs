using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour {
	public Slider slider;
	public Text curLevel;
	public Text nextLevel;

	static Level instance;
	public static Level Instance{
		get { return instance; }
	}   
	private void Awake()
	{
		instance = this;
	}

	void OnEnable(){
		UpdateLevel ();
		UpdateSlider (0);
	}


	public void UpdateLevel(){
		int level = PlayerPrefs.GetInt ("Level", 1);
		curLevel.text = level.ToString ();
		nextLevel.text = (level + 1).ToString ();
	}

	public void UpdateSlider(float value){
		slider.value = value;
	}
}
