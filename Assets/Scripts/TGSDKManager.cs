using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Together;

public class TGSDKManager : MonoBehaviour {

	public static string doubleID = "WcwXuvenxxXiNxHTc52";
	public static string forceID = "";
	public static string turnID = "";
	public static string trySkinID = "TZQAihaOQTVTYcF6qP1";
	public static string resetLevelID = "4UcrmFxAdYrQA52M1Lf";
	public static string resetDailyID = "fOyewGaJJI2xvOvxAut";
    public static string freeDiamondId = "";


    void Awake(){
		TGSDK.Initialize ("590d8Nf94oTc30109Ga1");
		TGSDK.PreloadAd ();
		TGSDK.PreloadAdFailedCallback = (string obj) => {
			TGSDK.PreloadAd();
		};
	}

}
