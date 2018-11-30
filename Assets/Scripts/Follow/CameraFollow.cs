using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour {
    [SerializeField]
    Transform target;

    [SerializeField]
    float smooth = 1;

    Vector3 offset;
    Vector3 rotationOffset;

    bool isDead = false;

    //摄像机是否在旋转
    bool isRotate = false;

    static CameraFollow instance;
    public static CameraFollow Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        offset = transform.position - target.position;
        rotationOffset = transform.eulerAngles - target.eulerAngles;
	}

    // Update is called once per frame
    void Update()
    {
        //位置跟随
        // Vector3 targetPos = new Vector3(0, target.position.y, target.position.z) + offset;
        Vector3 targetPos = target.position + offset;
        if (!isRotate)
        {
           // transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smooth);
        }
        //角度跟随
        //transform.RotateAround(target.position, Vector3.up, -1);
        if (PlayerController.Instance.GameState == 6)
        {
            if (!isDead)
            {
                StartCoroutine(ChangeSmooth());             
                isDead = true;
            }
        }
    }

    //摄像机跟随角色旋转
    public IEnumerator FollowPlayerRotation(float euler,float speed)
    {
        isRotate = true;
        float targetEuler = transform.eulerAngles.y + euler;

        //Debug.Log("transform.eulerAngles.y:" + transform.eulerAngles.y);
        //Debug.Log("euler:" + euler);
        //Debug.Log("targetEuler:" + targetEuler);
       
        float eulerY = transform.eulerAngles.y;
        while (eulerY<targetEuler)
        {
            eulerY += speed;
            transform.RotateAround(target.position, Vector3.up, speed);
            yield return null;
        }

        offset = transform.position - target.position;
        isRotate = false;
    }




    IEnumerator ChangeSmooth(){
        while(smooth<20){
            smooth++;
            yield return null;
        }
        smooth = 100;
    }
}
