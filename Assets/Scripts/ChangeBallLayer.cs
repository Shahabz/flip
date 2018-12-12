using UnityEngine;
using System.Collections;

public class ChangeBallLayer : MonoBehaviour {

    public int LayerOnEnter; // BallInHole
    public int LayerOnExit;  // BallOnTable


    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {	
			//进入黑洞时调整目标为不与地面碰撞
			other.gameObject.layer = LayerOnEnter;
			//设置状态为进入黑洞
			PlayerController.Instance.GameState = 6;
        }
    }
		
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
			//离开黑洞时调整目标可以与地面碰撞
            other.gameObject.layer = LayerOnExit;
			//设置状态为进入新关卡
			PlayerController.Instance.GameState = 7;
        }
    }
}