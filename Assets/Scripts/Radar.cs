using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Radar : MonoBehaviour {
    List<Transform> Ring;
	List<Transform> Hole;

//	[SerializeField]
	//BlackHole blackHole;

    [SerializeField]
    Transform player;

    int jumpCount;
    [HideInInspector]
    public int levelJumpCount;

	int ringIndex = -1;
	string lastRingName = "";
	string ringName = "";

	Vector3 ringPos;

    private void Start()
    {

        jumpCount = 0;
		//生成黑洞的间隔
		if (PlayerPrefs.GetInt ("Level", 1) < 3) {
			levelJumpCount = 1;
		} else if (PlayerPrefs.GetInt ("Level", 1) < 5) {
			levelJumpCount = 2;
		}else{
			levelJumpCount = 3;
		}
    }

    private void OnEnable()
    {
        Ring = new List<Transform>();
		Hole = new List<Transform>();
    }

    //雷达检测到可到达的环则改变主角面向，改变摄像机面向，生成环
    public void OnDisable()
    {
		
        if (Ring.Count != 0)
        {

			ringIndex = Random.Range (0, Ring.Count);
			ringName = Ring [ringIndex].name;

			//避免死循环
			int whileNum = 0;
			while ((ringName == lastRingName || Vector3.Distance(Ring[ringIndex].position,player.position)<1.5f)&& Ring.Count>1 ) {
				ringIndex = Random.Range (0, Ring.Count);
				ringName = Ring [ringIndex].name;
				whileNum++;
				if (whileNum > Ring.Count) {
					break;
				}
			}

			ringPos = Ring [ringIndex].position;
			lastRingName = ringName;



			if (jumpCount >= levelJumpCount && Hole.Count!=0)
            {				
				int holeIndex = Random.Range (0, Hole.Count);
				ringPos = Hole [holeIndex].position;
				if (Vector3.Distance (Hole [holeIndex].position, player.position) < 2.5f) {
					ringPos += new Vector3 (2.5f, 0, 2.5f);
				}
				Transform holeTrans = RingManager.Instance.GenerateHole(ringPos);

				//blackHole.hole_object = holeTrans;
				//blackHole.enabled = true;

				PlayerController.Instance.isHoling = true;
				PlayerController.Instance.holeTarget = holeTrans;
                jumpCount = 0;

				Level.Instance.UpdateSlider (1);
            }
            else
            {
				int size = 1;
				if (ringName.EndsWith ("small")) {
					size = 1;
				} else if (ringName.EndsWith ("mid")) {
					size = 2;
				} else if (ringName.EndsWith ("big")) {
					size = 3;
				}
				Transform ringTrans = RingManager.Instance.GenerateRings(ringPos,size);

				PlayerController.Instance.ringPos = ringPos;


				Perspective.Instance.StartCoroutine (Perspective.Instance.CheckObstacle (ringTrans.gameObject));

//				Transform holeTrans = RingManager.Instance.GenerateHole(ringPos);
//				blackHole.hole_object = holeTrans;
//				blackHole.enabled = true;
//				blackHole.StartCoroutine(ChangeIor ());
//				PlayerController.Instance.isHoling = true;
//				PlayerController.Instance.holeTarget = holeTrans;

				Level.Instance.UpdateSlider (jumpCount*1.0f/levelJumpCount);

				jumpCount++;


            }				

			Quaternion targetQuat = Quaternion.FromToRotation(Vector3.back, new Vector3((ringPos - player.position).x, 0, (ringPos - player.position).z));
			player.DORotateQuaternion(targetQuat, 0.5f);
			CameraRotate.Instance.xDeg = targetQuat.eulerAngles.y + 120;
        }
    }

//	IEnumerator ChangeIor(){
//		blackHole.ior = 0;
//		float time = 0;
//		while(blackHole.ior<6){
//			
//			blackHole.ior = Mathf.Lerp (0, 8, time);
//			time += Time.deltaTime;
//			yield return null;
//		}
//	}

	void GetHolePos(){
		foreach (Transform trans in Ring) {
			if (trans.name.EndsWith ("hole(Clone)")) {
				
			}
		}
	}


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "RingPos"){
            Ring.Add(other.transform);
			if (other.name.EndsWith ("hole")) {
				Hole.Add (other.transform);
			}
        }
    }
}
