using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Diamond : MonoBehaviour {
	public Text diamond;

	static Diamond instance;
	public static Diamond Instance
	{
		get { return instance; }
	}
	private void Awake()
	{
		instance = this;
	}

	void Start(){
		UpdateDiamond();
	}

	//增加存储的钻石及UI显示
	public void GetDiamond (int number){
		int curDiamond = PlayerPrefs.GetInt ("diamond", 0);
		curDiamond += number;
		diamond.text = curDiamond.ToString();
		PlayerPrefs.SetInt ("diamond", curDiamond);
	}

	//使用钻石，更新UI及存储，不够则弹出提示,返回购买状态
	public bool UseDiamond(int number){
		int curDiamond = PlayerPrefs.GetInt ("diamond", 0);
		if (curDiamond >= number) {
			curDiamond -= number;
			diamond.text = curDiamond.ToString();
			PlayerPrefs.SetInt ("diamond", curDiamond);
			return true;
		} else {
			TipPop.GenerateTip ("Not enough diamonds", 1f);
			return false;
		}
	}

	//刷新钻石信息
	public void UpdateDiamond(){
		int curDiamond = PlayerPrefs.GetInt ("diamond", 0);
		diamond.text = curDiamond.ToString();
	}
}
