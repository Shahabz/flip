using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class MultiHaptic {
	[DllImport("__Internal")]
	static extern void _hapticMedium();

	[DllImport("__Internal")]
	static extern void _hapticLight();

	[DllImport("__Internal")]
	static extern void _hapticHeavy();

	public static void HapticLight() {
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		if (PlayerPrefs.GetInt ("Taptic", 1) == 1) {
			_hapticLight ();
		}
	}

	public static void HapticMedium() {
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		if (PlayerPrefs.GetInt ("Taptic", 1) == 1) {
			_hapticMedium ();
		}
	}

	public static void HapticHeavy() {
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		if (PlayerPrefs.GetInt ("Taptic", 1) == 1) {
			_hapticHeavy ();
		}
	}
}