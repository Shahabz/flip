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
	}

	void Start(){

	}

	//存储相对于主角的环坐标序列
	public void SaveLevelPos(List<Vector3> posList){
		//可覆盖式存储
		int PosCount = posList.Count;
		for (int i = 0; i < PosCount; i++) {
			SaveVector3 (posList [i], i);
		}
		//记录下当前生成环的次数，进入新关卡需要清空
		int curMapIndex = PlayerPrefs.GetInt ("CurMap", 0);
		if (posList.Count > PlayerPrefs.GetInt ("PosCount"+curMapIndex, 0))
			PlayerPrefs.SetInt ("PosCount"+curMapIndex, posList.Count);
	}

	//读取所存储的列表
	public List<Vector3> ReadLevelPos(){
		int curMapIndex = PlayerPrefs.GetInt ("CurMap", 0);
		int PosCount = PlayerPrefs.GetInt ("PosCount"+curMapIndex, 0);
		List<Vector3> posList = new List<Vector3> ();
		for (int i = 0; i < PosCount; i++) {
			posList.Add (ReadVector3 (i));
		}
		return posList;
	}

	//清空列表
	public void ClearPosList(){
		int curMapIndex = PlayerPrefs.GetInt ("CurMap", 0);
		PlayerPrefs.SetInt ("PosCount"+curMapIndex, 0);
	}

	//配合列表存储使用
	void SaveVector3(Vector3 v3,int index){
		int curMapIndex = PlayerPrefs.GetInt ("CurMap", 0);
		PlayerPrefs.SetFloat ("LevelPosX" + index+curMapIndex, v3.x);
		PlayerPrefs.SetFloat ("LevelPosY" + index+curMapIndex, v3.y);
		PlayerPrefs.SetFloat ("LevelPosZ" + index+curMapIndex, v3.z);
	}
	//配合列表读取使用
	Vector3 ReadVector3(int index){
		int curMapIndex = PlayerPrefs.GetInt ("CurMap", 0);
		float x = PlayerPrefs.GetFloat ("LevelPosX" + index+curMapIndex, 0);
		float y = PlayerPrefs.GetFloat ("LevelPosY" + index+curMapIndex, 0);
		float z = PlayerPrefs.GetFloat ("LevelPosZ" + index+curMapIndex, 0);
		return new Vector3 (x, y, z);
	}

	//存储三维坐标
	public void SetVector3(string name,Vector3 v3){
		int curMapIndex = PlayerPrefs.GetInt ("CurMap", 0);
		PlayerPrefs.SetFloat (name + "X"+curMapIndex, v3.x);
		PlayerPrefs.SetFloat (name + "Y"+curMapIndex, v3.y);
		PlayerPrefs.SetFloat (name + "Z"+curMapIndex, v3.z);
	}
	//读取三维坐标
	public Vector3 GetVector3(string name){
		int curMapIndex = PlayerPrefs.GetInt ("CurMap", 0);
		float x = PlayerPrefs.GetFloat (name + "X"+curMapIndex, 0);
		float y = PlayerPrefs.GetFloat (name + "Y"+curMapIndex, 0);
		float z = PlayerPrefs.GetFloat (name + "Z"+curMapIndex, 0);
		return new Vector3 (x, y, z);
	}
}
