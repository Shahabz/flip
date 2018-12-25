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

	//关卡进度显示时刷新进度和关卡数字
	void OnEnable(){
		UpdateLevel ();
		UpdateSlider (0);
	}

	//刷新关卡数
	public void UpdateLevel(){
		int curMapIndex = PlayerPrefs.GetInt ("CurMap", 0);
		int level = PlayerPrefs.GetInt ("Level"+curMapIndex, 1);
		curLevel.text = level.ToString ();
		nextLevel.text = (level + 1).ToString ();
	}

	//刷新关卡进度
	public void UpdateSlider(float value){
		slider.value = value;
	}
}
