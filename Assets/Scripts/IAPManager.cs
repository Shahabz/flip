using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using Together;

public class IAPManager : MonoBehaviour
{
	private static IAPManager instance;
	public static IAPManager Instance {
		get{ return instance; }
	}
	void Awake ()
	{
		instance = this;
	}



	void Start(){
		
	}
		
	public void OnPurchaseFinish (Product product)
	{

		if (product != null) {
			
			Debug.Log ("Purchase:" + product.definition.id);
			if (product.definition.id == "flippy_noads") {				
				PlayerPrefs.SetInt("no_ads", 1);
			}
			if(product.definition.id == "flippy_bundle"){
				Diamond.Instance.GetDiamond (900);
				PlayerPrefs.SetInt("no_ads", 1);
				PlayerPrefs.SetInt ("streetman", 1);
				PlayerPrefs.SetInt ("flippy_bundle", 1);
			}
			if(product.definition.id == "flippy_0.99"){
				Diamond.Instance.GetDiamond (275);
			}
			if(product.definition.id == "flippy_4.99"){
				Diamond.Instance.GetDiamond (1450);
			}
			if(product.definition.id == "flippy_9.99"){
				Diamond.Instance.GetDiamond (3200);
			}
			if(product.definition.id == "flippy_19.99"){
				Diamond.Instance.GetDiamond (6600);
			}
				
		}
	}

	public void OnPurchaseFailed (Product product, PurchaseFailureReason reason)
	{
		if (product != null) {
			Debug.Log ("fiailed:" + product.definition.id + " reason:" + reason);
		}
	}
		
	public void OnBackBtn(){
		gameObject.SetActive (false);
	}

	//看广告免费得钻石
	public void OnFreeBtn(){
		if (TGSDK.CouldShowAd (TGSDKManager.freeDiamondId)) {
			TGSDK.ShowAd (TGSDKManager.freeDiamondId);
			TGSDK.AdCloseCallback = (string obj) => {
				Diamond.Instance.GetDiamond (25);
			};
		} else {
			TipPop.GenerateTip ("no ads", 0.5f);
		}
	}
}

