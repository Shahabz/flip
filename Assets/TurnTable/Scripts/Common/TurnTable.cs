using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TurnTable : MonoBehaviour {

	//获取转盘旋转脚本
	public Rotation rotation;
	//获取指针检测脚本
	public Needle needle;
	//获取转盘元素集合
	public GameObject turnTable;
	public GameObject turnBtn;
	public GameObject awesome;
	public Text aresomeText;
	//转盘是否结束旋转
	bool isFinish = false;
	//转盘元素金币数
	int[] diamonds;

	//初始化
	void OnEnable () {
		rotation.RotationFinish +=()=>{ 
			RotateFinish ();
			rotation.RotateLittle ();
		};
		needle.CheckNeedleCallBack += (Collider2D coll) => {
			CheckNeedle(coll);
		};

		diamonds = new int[]{ 1, 50, 10, 100, 5, 20 };
	}

	//点击广告旋转按钮
	public void OnTurnBtn(){
		rotation.RotateThis();
	}

	//转盘结束回调
	void RotateFinish(){
		isFinish = true;
	}

	int diamond = 0;
	//转盘结束时获得对应奖励
	void CheckNeedle(Collider2D coll){
		if (isFinish) {
			if (coll.name.StartsWith ("item")) {
				int index = int.Parse(coll.name.Split (new char[]{ 'm' }) [1]);
				diamond = diamonds [index];
				isFinish = false;

				turnTable.SetActive (false);
				aresomeText.text = diamond.ToString ();
				awesome.SetActive (true);
			}else{
				rotation.RotateLittle ();
			}

		}
	}

	//点击领取按钮
	public void OnAwesomeBtn(){	
		Diamond.Instance.GetDiamond (diamond);
		MoneyManager.Instance.UpdateDiamond ();
		PlayerPrefs.SetInt ("NewDayTurn", 0);
		turnBtn.SetActive(false);
		gameObject.SetActive (false);
	}

}
