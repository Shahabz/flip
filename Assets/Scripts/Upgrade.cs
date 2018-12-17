using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Upgrade : MonoBehaviour {
	public Text speedLevel;
	public Text coinLevel;
	public Text offlineLevel;
	public Text speedPrice;
	public Text coinPrice;
	public Text offlinePrice;
	int speedLvInt = 0;
	int coinLvInt = 0;
	int offlineLvInt = 0;
	int speedPriceInt = 0;
	int coinPriceInt = 0;
	int offlinePriceInt = 0;

	public Transform speedTrans;
	public Transform coinTrans;
	public Transform offlineTrans;


	public void OnSpeedBtn(){
		if (Gold.Instance.UseGold (speedPriceInt)) {
			UpdateGold ();
			speedLvInt++;
			speedLevel.text = speedLvInt.ToString ();
			PlayerPrefs.SetInt ("speedLevel", speedLvInt);
			speedPriceInt = (int)(speedPriceInt * 1.05f);
			speedPrice.text = speedPriceInt.ToString ();
			PlayerPrefs.SetInt ("speedPrice", speedPriceInt);
			speedTrans.DOScale (1.1f, 0.15f).OnComplete (() => {
				speedTrans.DOScale (1f, 0.15f);
			});
		} else {
			speedTrans.DOShakePosition (0.15f, 1, 10, 90, false, true);
		}
	}

	public void OnCoinBtn(){
		if (Gold.Instance.UseGold (coinPriceInt)) {
			UpdateGold ();
			coinLvInt++;
			coinLevel.text = coinLvInt.ToString ();
			PlayerPrefs.SetInt ("coinLevel", coinLvInt);
			coinPriceInt = (int)(coinPriceInt * 1.05f);
			coinPrice.text = coinPriceInt.ToString ();
			PlayerPrefs.SetInt ("coinPrice", coinPriceInt);
			coinTrans.DOScale (1.1f, 0.15f).OnComplete (() => {
				coinTrans.DOScale (1f, 0.15f);
			});
		} else {
			coinTrans.DOShakePosition (0.15f, 1, 10, 90, false, true);
		}
	}

	public void OnOfflineBtn(){
		if (Gold.Instance.UseGold (offlinePriceInt)) {
			UpdateGold ();
			offlineLvInt++;
			offlineLevel.text = offlineLvInt.ToString ();
			PlayerPrefs.SetInt ("offlineLevel", offlineLvInt);
			offlinePriceInt = (int)(offlinePriceInt * 1.05f);
			offlinePrice.text = offlinePriceInt.ToString ();
			PlayerPrefs.SetInt ("offlinePrice", offlinePriceInt);
			offlineTrans.DOScale (1.1f, 0.15f).OnComplete (() => {
				offlineTrans.DOScale (1f, 0.15f);
			});
		} else {
			offlineTrans.DOShakePosition (0.15f, 1, 10, 90, false, true);
		}
	}

	public void UpdateText(){
		speedLvInt = PlayerPrefs.GetInt ("speedLevel", 1);
		coinLvInt = PlayerPrefs.GetInt ("coinLvInt", 1);
		offlineLvInt = PlayerPrefs.GetInt ("offlineLvInt", 1);
		speedPriceInt = PlayerPrefs.GetInt ("speedPrice", 281);
		coinPriceInt = PlayerPrefs.GetInt ("coinPrice", 281);
		offlinePriceInt = PlayerPrefs.GetInt ("offlinePrice", 281);

		speedLevel.text = "LV" + speedLvInt;
		coinLevel.text = "LV" + coinLvInt;
		offlineLevel.text = "LV" + offlineLvInt;
		speedPrice.text = "$" + speedPriceInt;
		coinPrice.text = "$" + coinPriceInt;
		offlinePrice.text = "$" + offlinePriceInt;
	}

	public Text gold;
	void UpdateGold(){
		int curGold = PlayerPrefs.GetInt ("Gold", 0);
		gold.text = curGold.ToString();
	}

	[SerializeField]
	Transform moveBtn;
	bool moveFinish = true;
	public void OnMoveBtn(){
		if (moveFinish) {
			moveFinish = false;
			if (moveBtn.eulerAngles.z == 0) {
				transform.DOMoveX (transform.position.x + 172, 0.5f, false).OnComplete (() => {
					moveBtn.eulerAngles = new Vector3 (0, 0, 180);
					moveFinish = true;
				});
			} else if (moveBtn.eulerAngles.z == 180) {
				UpdateText ();

				transform.DOMoveX (transform.position.x - 172, 0.5f, false).OnComplete (() => {
					moveBtn.eulerAngles = new Vector3 (0, 0, 0);
					moveFinish = true;
				});
			}
		}
	}
}
