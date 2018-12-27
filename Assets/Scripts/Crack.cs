using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crack : MonoBehaviour {
	public GameObject crack;
	public InputField level1;
	public InputField level2;
	public InputField level3;

	void Start(){
		level1.onEndEdit.AddListener ((string text)=>{			
			PlayerPrefs.SetInt("Level0",int.Parse(text));
		});
		level2.onEndEdit.AddListener ((string text)=>{
			PlayerPrefs.SetInt("Level1",int.Parse(text));
		});
		level3.onEndEdit.AddListener ((string text)=>{
			PlayerPrefs.SetInt("Level2",int.Parse(text));
		});
	}

	public void OnCrackBtn(){
		crack.SetActive (!crack.activeSelf);
	}


}
