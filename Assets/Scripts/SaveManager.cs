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

	//配合列表存储使用
	void SaveVector3(Vector3 v3,int index){
		PlayerPrefs.SetFloat ("LevelPosX" + index, v3.x);
		PlayerPrefs.SetFloat ("LevelPosY" + index, v3.y);
		PlayerPrefs.SetFloat ("LevelPosZ" + index, v3.z);
	}
	//配合列表读取使用
	Vector3 ReadVector3(int index){
		float x = PlayerPrefs.GetFloat ("LevelPosX" + index, 0);
		float y = PlayerPrefs.GetFloat ("LevelPosY" + index, 0);
		float z = PlayerPrefs.GetFloat ("LevelPosZ" + index, 0);
		return new Vector3 (x, y, z);
	}

	//存储三维坐标
	public void SetVector3(string name,Vector3 v3){
		PlayerPrefs.SetFloat (name + "X", v3.x);
		PlayerPrefs.SetFloat (name + "Y", v3.y);
		PlayerPrefs.SetFloat (name + "Z", v3.z);
	}
	//读取三维坐标
	public Vector3 GetVector3(string name){
		float x = PlayerPrefs.GetFloat (name + "X", 0);
		float y = PlayerPrefs.GetFloat (name + "Y", 0);
		float z = PlayerPrefs.GetFloat (name + "Z", 0);
		return new Vector3 (x, y, z);
	}
}
