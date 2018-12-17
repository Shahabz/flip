using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TipPop : MonoBehaviour {

	//需要将文字tip放入Resources目录
	public static void GenerateTip(string str,float time){
		GameObject tip = Resources.Load ("tip") as GameObject;
		GameObject tipObj = Instantiate (tip, GameObject.Find ("Canvas").transform);
		Text text = tipObj.GetComponent<Text>();
		text.text = str;
		text.transform.DOScale (1.5f, time);
		text.transform.DOLocalMove (Vector3.up * 100, time, false).OnComplete(()=>{
			Destroy(text.gameObject);
		});
	}

	public static void GenerateTip(string str, float time,Color color)
	{
		GameObject tip = Resources.Load("tip") as GameObject;
		GameObject tipObj = Instantiate(tip, GameObject.Find("Canvas").transform);
		Text text = tipObj.GetComponent<Text>();
		text.text = str;
		text.color = color;
		text.transform.DOScale(1.5f, time);
		text.transform.DOLocalMove(Vector3.up * 100, time, false).OnComplete(() => {
			Destroy(text.gameObject);
		});
	}
		
	public static void GenerateTipStay(string str, float time,Color color)
	{		
		
		GameObject tip = Resources.Load("tip") as GameObject;
		GameObject tipObj = Instantiate(tip, GameObject.Find("Canvas").transform);
		PlayerController.Instance.tempText = tipObj;
		Text text = tipObj.GetComponent<Text>();
		text.text = str;
		PlayerController.Instance.tempTextStr = str;
		text.color = color;
		text.transform.DOScale(1.5f, time);
		text.transform.DOLocalMove(Vector3.up * 150, time, false).OnComplete(() => {

		});
	}

}
