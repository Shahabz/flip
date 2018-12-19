using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoxBow : MonoBehaviour {

	[SerializeField]
	Transform upPostion;
	Rigidbody playerRig;

	public void BoxBoon(){
		//transform.DOShakeScale (0.5f, 1.5f, 10, 90, true).SetEase(Ease.InBack).OnComplete(()=>{
			transform.DOScale(new Vector3(3f,0.2f,3f),1.2f).OnComplete(()=>{
				transform.DOScale(new Vector3(1.5f,0.6f,0.5f),0.15f).OnComplete(()=>{
					Destroy(gameObject);
					FlyGold.Instance.GenerateGold(20,transform.position);
					PlayerController.Instance.ShowDoubleD(0.2f);
				});
			});
		//});
	}



	void OnTriggerEnter(Collider coll){
		if (coll.name == "playerRagdoll") {
			StartCoroutine (FollowBox (coll.transform));
			playerRig = coll.GetComponent<Rigidbody> ();
			BoxBoon ();
		}
	}

	IEnumerator FollowBox(Transform player){
		while (transform) {
			player.position = upPostion.position;
			yield return null;
		}
	}
}
