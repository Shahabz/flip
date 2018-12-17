using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needle : MonoBehaviour {
	public delegate void CheckNeedle(Collider2D coll);
	public event CheckNeedle CheckNeedleCallBack;

	void OnTriggerStay2D(Collider2D coll){		
		if (CheckNeedleCallBack != null) {
			CheckNeedleCallBack (coll);
		}
	}

}
