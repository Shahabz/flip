using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OfflineReward : MonoBehaviour {

	public GameObject turnTable;

	DateTime currentDate;
	DateTime oldDate;

	void Awake(){
		StartCoroutine (GetNetWorkTime ());
	}

	void Start(){
		Invoke ("NewDayTurn", 1);
	}

	//退出游戏获得存储离线时间
	void OnApplicationQuit()
	{
		PlayerPrefs.SetString("offlineTime", System.DateTime.Now.ToBinary().ToString());
		PlayerPrefs.SetInt ("AutoDaily", 1);
	}

	//离开游戏存储离线时间，进入游戏获得离线奖励
	void OnApplicationPause(bool isPause){
		if (isPause) {
			PlayerPrefs.SetString("offlineTime", System.DateTime.Now.ToBinary().ToString());
		} else {
			GetReward ();
		}
	}

	//获得离线奖励或者弹出离线奖励窗口
	void GetReward(){
		
	}

	//获得离线的分钟数
	int OfflineTime(){
		//Store the current time when it starts
		currentDate = System.DateTime.Now;
		//Grab the old time from the player prefs as a long
		long temp = Convert.ToInt64(PlayerPrefs.GetString("sysString",currentDate.ToBinary().ToString()));
		//Convert the old time from binary to a DataTime variable
		DateTime oldDate = DateTime.FromBinary(temp);
		//Use the Subtract method and store the result as a timespan variable
		TimeSpan difference = currentDate.Subtract(oldDate);
		return difference.Minutes;
	}
		
	//新的一天则刷新每日物品
	private IEnumerator GetNetWorkTime(){
		WWW req = new WWW ("http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=2");
		yield return req;

		if (req.error == null) {
			int lastDay = PlayerPrefs.GetInt ("DayOfYear", 0);
			string timeStamp = req.text.Split ('=') [1].Substring (0, 10); 
			DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime (new DateTime (1970, 1, 1));
			long lTime = long.Parse (timeStamp + "0000000");
			TimeSpan toNow = new TimeSpan (lTime);
			DateTime now = dtStart.Add (toNow);
			if (now.DayOfYear - lastDay> 0) {
				PlayerPrefs.SetInt ("DayOfYear", now.DayOfYear);
				//这里标记每日需要处理的物品
				PlayerPrefs.SetInt ("NewDayTurn", 1);
				PlayerPrefs.SetInt ("Mission1", 1);
				PlayerPrefs.SetInt ("Mission2", 1);
				PlayerPrefs.SetInt ("Mission3", 1);
				PlayerPrefs.SetInt ("Mission4", 1);
				PlayerPrefs.SetInt ("freeMission", 0);
				PlayerPrefs.SetInt ("CarHit",0);
				PlayerPrefs.SetInt ("GloveHit",0);
				PlayerPrefs.SetInt ("Roof", 0);
				PlayerPrefs.SetInt ("Road", 0);
				PlayerPrefs.SetInt ("CarUp", 0);
				for (int i = 1; i < 4; i++) {
					PlayerPrefs.SetString ("Mission_name" + i, "null");
					//PlayerPrefs.SetInt ("Mission_number" + i, 0);
				}
			}
		}
		else {
			yield return new WaitForSeconds(300);

			StartCoroutine (GetNetWorkTime ());
		}

	}

	void NewDayTurn(){
		if (PlayerPrefs.GetInt ("NewDayTurn", 0) == 1) {			
			turnTable.SetActive (true);
		}
	}
		
}
