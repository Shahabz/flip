using UnityEngine;
using System.Collections;

public class CameraGuide : MonoBehaviour
{
    public Transform target;
    //public Transform targetTO;
    public Vector3 targetOffset;
    float distance = 5.0f;
    //public float maxDistance = 20;//缩放
    //public float minDistance = .6f;//缩放
    //public float xSpeed = 200.0f;//速度
    //public float ySpeed = 200.0f;
    //public int yMinLimit = -80;//限定角度
    //public int yMaxLimit = 80;
    //public int zoomRate = 40;
    public float zoomDampening = 5.0f;
    public float xDeg = 0.0f;//自身的角度记录
    public float yDeg = 0.0f;//自身的角度
    public float currentDistance;//缩放记录
    public float desiredDistance;//缩放
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    public Quaternion rotation;
    private Vector3 position;

   // public float X;//初始化角度X轴
   // public float Y;//初始化角度Y轴
    /// <summary>
    /// 初始化距离
    /// </summary>
    public float CameDistance;

    void Start()
    {
        Init();
        desiredDistance = CameDistance;
    }
		
    public void Init()
    {
        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;
       
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        //xDeg = Vector3.Angle(Vector3.right, transform.right)-120;
		xDeg = 25;
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }


    void LateUpdate()
    {			
		// 设置相机旋转
		desiredRotation = Quaternion.Euler (yDeg, xDeg, 0);
		currentRotation = transform.rotation;

		rotation = Quaternion.Lerp (currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
		transform.rotation = rotation;

		position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
		transform.position = position;

	}

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
