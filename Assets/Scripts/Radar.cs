using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Radar : MonoBehaviour {
    List<Transform> Ring;
	List<Transform> Hole;

	//获得当前添加的位置
	[HideInInspector]
	public List<Vector3> levelPos = new List<Vector3> ();
	//获取已存储的位置
	List<Vector3> curLevelPos;

    [SerializeField]
    Transform player;
	[HideInInspector]
	public Transform city;

	[HideInInspector]
	public int jumpCount;
    [HideInInspector]
    public int levelJumpCount;
	[HideInInspector]
	public int floorNumber = 0;
	[HideInInspector]
	public Vector3[] curLevelRings;
	[HideInInspector]
	public Transform startPos;

	int ringIndex = -1;
	string lastRingName = "";
	string ringName = "";

	Vector3 ringPos;

	SaveManager saveManager;

    private void Start()
    {
		jumpCount = 0;
//		//通过环数量进行难度控制
//		if (PlayerPrefs.GetInt ("Level", 1) < 3) {
//			levelJumpCount = 1;
//		} else if (PlayerPrefs.GetInt ("Level", 1) < 5) {
//			levelJumpCount = 2;
//		}else{
//			levelJumpCount = 3;
//		}
    }

	//开始扫描
    private void OnEnable()
    {
		
		saveManager = SaveManager.Instance;
     // Ring = new List<Transform>();
	 //	Hole = new List<Transform>();
		//设置重生点
		if (levelPos.Count == 0) {
			PlayerController.Instance.ReGamePos = player.position;
		}
		//获取存储好的位置
		curLevelPos = saveManager.ReadLevelPos ();
    }

    //雷达检测到可到达的环则改变主角面向，改变摄像机面向，生成环
    public void OnDisable()
    {
		//有环
        //if (Ring.Count != 0)
        //{
			//随机取其中一个并获得名字
			//ringIndex = Random.Range (0, Ring.Count);
			//ringName = Ring [ringIndex].name;

			//避免死循环
			//多个环时避免重复获取到环
			//int whileNum = 0;
			//while ((ringName == lastRingName || Vector3.Distance(Ring[ringIndex].position,player.position)<1.5f)&& Ring.Count>1 ) {
			//	ringIndex = Random.Range (0, Ring.Count);
			//	ringName = Ring [ringIndex].name;
			//	whileNum++;
			//	if (whileNum > Ring.Count) {
			//		break;
			//	}
		//	}

			//获得环的位置信息，保存最后生成环名字
		ringPos = curLevelRings[jumpCount]+startPos.position;
		//	lastRingName = ringName;

			//环数量到达上线且附近有黑洞则生成黑洞
		if (jumpCount >= curLevelRings.Length-1) {				
			//int holeIndex = Random.Range (0, Hole.Count);
			//ringPos = curLevelRings [jumpCount]+;
			//如果黑洞距离人太近则偏移一段距离
			//if (Vector3.Distance (Hole [holeIndex].position, player.position) < 2.5f) {
			//	ringPos += new Vector3 (2.5f, 0, 2.5f);
			//}	

			//如果当前位置已存储，则使用已存储的位置
			if (curLevelPos.Count > jumpCount) {	
				ringPos = curLevelPos [jumpCount] + city.position;
			}
					
			Transform holeTrans = RingManager.Instance.GenerateHole (ringPos);

			//保存黑洞位置偏移
			levelPos.Add (ringPos - city.position);
			saveManager.SaveLevelPos (levelPos);

			//透视黑洞
			//Perspective.Instance.StartCoroutine (Perspective.Instance.CheckObstacle (holeTrans.gameObject));

			//将黑洞传递给玩家控制脚本
			PlayerController.Instance.holeTarget = holeTrans;
			//清空环数量统计
			jumpCount = 0;
			//刷新关卡数据
			Level.Instance.UpdateSlider (1);
		} else {
			//获取环的尺寸
			int size = 1;
			if (ringName.EndsWith ("small")) {
				size = 1;
			} else if (ringName.EndsWith ("mid")) {
				size = 2;
			} else if (ringName.EndsWith ("big")) {
				size = 3;
			}

			//如果当前位置已存储，则使用已存储的位置
			if (curLevelPos.Count > jumpCount) {
				ringPos = curLevelPos [jumpCount] + city.position;
			}				

			Transform ringTrans = RingManager.Instance.GenerateRings (ringPos, size);

			//传递环对象
			PlayerController.Instance.ringPos = ringPos;
			//保存环偏移
			levelPos.Add (ringPos - city.position);
			saveManager.SaveLevelPos (levelPos);
			//透视环
			//Perspective.Instance.StartCoroutine (Perspective.Instance.CheckObstacle (ringTrans.gameObject));

			Level.Instance.UpdateSlider (jumpCount * 1.0f / (curLevelRings.Length-1));

			jumpCount++;
		}			

			//获取主角正确的面向并旋转
		Quaternion targetQuat = Quaternion.FromToRotation(Vector3.back, new Vector3((ringPos - player.position).x, 0, (ringPos - player.position).z));
		if (targetQuat.x == 1) {
			targetQuat = Quaternion.Euler (new Vector3 (0, targetQuat.eulerAngles.y, 0));
			Debug.Log ("ResetPlayerPostion");
		}
		player.DORotateQuaternion(targetQuat, 0.5f).OnComplete(()=>{
			PlayerController.Instance.finishBackRing = true;
			//PlayerController.Instance.isRotate = false;
		});
			//旋转摄像机
		CameraRotate.Instance.xDeg = targetQuat.eulerAngles.y + 120;
       // }
		//CheckObstacle();
	}

	void CheckObstacle(){
		RaycastHit[] hit;  
		Vector3 transPos = transform.position;

		Vector3 targetPos = player.transform.position + new Vector3 (0, 1f, 0);
		hit = Physics.RaycastAll (transPos, (targetPos - transPos), Vector3.Distance (transPos, targetPos));  

		if (hit.Length > 1) {  
			for (int i = 0; i < hit.Length; i++) {
				print (hit [i].collider.name);
			}
			CameraRotate.Instance.xDeg += 120;
		} 
	}
}
