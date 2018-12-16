using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleUI : MonoBehaviour {

	public void OnButtonClick(){
		transform.DOScale (1.2f, 0.15f).OnComplete (()=>{
			transform.DOScale (1f, 0.15f);
		});
	}
}
