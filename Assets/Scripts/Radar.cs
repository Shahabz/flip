using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Radar : MonoBehaviour {
    List<Transform> Ring;

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

            Quaternion targetQuat = Quaternion.FromToRotation(Vector3.back, new Vector3((ringPos - player.position).x, 0, (ringPos - player.position).z));
            player.DORotateQuaternion(targetQuat, 0.5f);
            //CameraFollow.Instance.StartCoroutine(CameraFollow.Instance.FollowPlayerRotation(targetQuat.eulerAngles.y, 2));
            CameraRotate.Instance.xDeg = targetQuat.eulerAngles.y + 120;
            //Debug.Log(targetQuat.eulerAngles);
            if (jumpCount >= levelJumpCount)
            {
				Transform holeTrans = RingManager.Instance.GenerateHole(ringPos);
				PlayerController.Instance.isHoling = true;
				PlayerController.Instance.holeTarget = holeTrans;
                jumpCount = 0;
            }
            else
            {
                RingManager.Instance.GenerateRings(ringPos);
				jumpCount++;
            }
            
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "RingPos"){
            Ring.Add(other.transform);
        }
    }
}
