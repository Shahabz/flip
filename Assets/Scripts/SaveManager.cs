using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {
	static SaveManager instance;
	public static SaveManager Instance{
		get { return instance; }
	}   
	private void Awake()
	{
		instance = this;
	//	ClearPosList ();
	}

	void Start(){

	}

	//存储玩家所处关卡位置
	//列表首位为起始位置，第二位为对应第一层的起始位置，之后为目标位置与主角的偏移量
	//存储列表及列表曾经最大数量；
	public void SaveLevelPos(List<Vector3> posList){
		int PosCount = posList.Count;
		for (int i = 0; i < PosCount; i++) {
			SaveVector3 (posList [i], i);
		}

		if (posList.Count > PlayerPrefs.GetInt ("PosCount", 0))
			PlayerPrefs.SetInt ("PosCount", posList.Count);
	}

	//读取所存储的列表
	public List<Vector3> ReadLevelPos(){
		
		int PosCount = PlayerPrefs.GetInt ("PosCount", 0);

		List<Vector3> posList = new List<Vector3> ();
		for (int i = 0; i < PosCount; i++) {
			posList.Add (ReadVector3 (i));
		}
		return posList;
	}

	//清空列表
	public void ClearPosList(){
		PlayerPrefs.SetInt ("PosCount", 0);
	}

	//存储坐标信息
	void SaveVector3(Vector3 v3,int index){
		PlayerPrefs.SetFloat ("LevelPosX" + index, v3.x);
		PlayerPrefs.SetFloat ("LevelPosY" + index, v3.y);
		PlayerPrefs.SetFloat ("LevelPosZ" + index, v3.z);
	}

	//读取坐标信息
	Vector3 ReadVector3(int index){
		float x = PlayerPrefs.GetFloat ("LevelPosX" + index, 0);
		float y = PlayerPrefs.GetFloat ("LevelPosY" + index, 0);
		float z = PlayerPrefs.GetFloat ("LevelPosZ" + index, 0);
		return new Vector3 (x, y, z);
	}
}
