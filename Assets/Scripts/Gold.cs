using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gold : MonoBehaviour {
	public Text gold;

	static Gold instance;
	public static Gold Instance
	{
		get { return instance; }
	}
	private void Awake()
	{
		instance = this;
	}

	void Start(){	
		//GetGold (9999999);	
		UpdateGold();
	}

	//增加存储的钻石及UI显示
	public void GetGold (int number){
		int curGold = PlayerPrefs.GetInt ("Gold", 0);
		curGold += number;
		gold.text = curGold.ToString();
		PlayerPrefs.SetInt ("Gold", curGold);
	}

	//使用金币，更新UI及存储，不够则弹出提示,返回购买状态
	public bool UseGold(int number){
		int curGold = PlayerPrefs.GetInt ("Gold", 0);
		if (curGold >= number) {
			curGold -= number;
			gold.text = curGold.ToString();
			PlayerPrefs.SetInt ("Gold", curGold);
			return true;
		} else {
			TipPop.GenerateTip ("Not enough Gold", 1f,Color.black);
			return false;
		}
	}

	//刷新钻石信息
	public void UpdateGold(){
		int curGold = PlayerPrefs.GetInt ("Gold", 0);
		gold.text = curGold.ToString ();
	}
}
