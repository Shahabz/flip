using UnityEngine;
using System.Collections;

public class ChangeBallLayer : MonoBehaviour {

    public int LayerOnEnter; // BallInHole
    public int LayerOnExit;  // BallOnTable


    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {			
			other.gameObject.layer = LayerOnEnter;
			PlayerController.Instance.GameState = 6;
        }
    }
		
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.layer = LayerOnExit;
			PlayerController.Instance.GameState = 7;
        }
    }
}