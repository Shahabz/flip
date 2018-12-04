using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Radar : MonoBehaviour {
    List<Transform> Ring;
	List<Transform> Hole;

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
        levelJumpCount = 1;
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
			while (ringName == lastRingName || Vector3.Distance(Ring[ringIndex].position,player.position)<1.5f ) {
				ringIndex = Random.Range (0, Ring.Count);
				ringName = Ring [ringIndex].name;
			}

			ringPos = Ring [ringIndex].position;
			lastRingName = ringName;

			if (jumpCount >= levelJumpCount && Hole.Count!=0)
            {				
				int holeIndex = Random.Range (0, Hole.Count);
				ringPos = Hole [holeIndex].position;
				if (Vector3.Distance (Hole [holeIndex].position, player.position) < 1.5f) {
					ringPos += new Vector3 (1.5f, 0, 0);
				}
				Transform holeTrans = RingManager.Instance.GenerateHole(ringPos);
				PlayerController.Instance.isHoling = true;
				PlayerController.Instance.holeTarget = holeTrans;
                jumpCount = 0;
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
				RingManager.Instance.GenerateRings(ringPos,size);
				jumpCount++;
            }


			Quaternion targetQuat = Quaternion.FromToRotation(Vector3.back, new Vector3((ringPos - player.position).x, 0, (ringPos - player.position).z));
			player.DORotateQuaternion(targetQuat, 0.5f);
			CameraRotate.Instance.xDeg = targetQuat.eulerAngles.y + 120;
        }
    }

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
