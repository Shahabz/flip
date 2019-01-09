using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdaptSkill : MonoBehaviour {
	public Transform skill;
	// Use this for initialization
	void Start () {
		if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX ||
		   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXR ||
		   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXS ||
		   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXSMax) {
			skill.localPosition += new Vector3 (70,0,0);
		}
	}
	

}
