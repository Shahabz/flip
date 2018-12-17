using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour {
	public Text[] golds;
	public Text[] diamonds;

	static MoneyManager instance;
	public static MoneyManager Instance
	{
		get { return instance; }
	}
	private void Awake()
	{
		instance = this;
	}

	public void UpdateGold(){
		int curGold = PlayerPrefs.GetInt ("Gold", 0);
		for (int i = 0; i < golds.Length; i++) {
			golds [i].text = curGold.ToString ();
		}

	}

	public void UpdateDiamond(){
		int curDiamond = PlayerPrefs.GetInt ("diamond", 0);
		for (int i = 0; i < diamonds.Length; i++) {
			diamonds [i].text = curDiamond.ToString ();
		}
	}

}
