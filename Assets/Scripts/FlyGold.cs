using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlyGold : MonoBehaviour {
	[SerializeField]
	GameObject gold;
	[SerializeField]
	Transform player;

	static FlyGold instance;
	public static FlyGold Instance
	{
		get { return instance; }
	}
	private void Awake()
	{
		instance = this;
	}

	public void GenerateGold(int number,Vector3 goldPos){
		for (int i = 0; i < number; i++) {
			GameObject go = Instantiate (gold, goldPos + new Vector3 (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f)),
				Quaternion.Euler (new Vector3 (Random.Range (0f, 360f), Random.Range (0f, 360f), Random.Range (0f, 360f))),transform);
			go.GetComponent<Rigidbody> ().AddExplosionForce (500, goldPos, 10);
			DestroyGold (go);
		}
		player.DOLocalMoveY (player.position.y - 1, 0.5f, false).SetEase (Ease.InOutBack);
	}

	void DestroyGold(GameObject go){
		Destroy (go, Random.Range(3f,5f));
	}

	public void GenerateGoldGuide(int number,Vector3 goldPos){
		for (int i = 0; i < number; i++) {
			GameObject go = Instantiate (gold, goldPos + new Vector3 (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f)),
				Quaternion.Euler (new Vector3 (Random.Range (0f, 360f), Random.Range (0f, 360f), Random.Range (0f, 360f))),transform);
			go.GetComponent<Rigidbody> ().AddExplosionForce (500, goldPos, 10);
			DestroyGold (go);
		}
	}

	public void GenerateGoldNoColl(int number,Vector3 goldPos){
		for (int i = 0; i < number; i++) {
			GameObject go = Instantiate (gold, goldPos + new Vector3 (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f)),
				Quaternion.Euler (new Vector3 (Random.Range (0f, 360f), Random.Range (0f, 360f), Random.Range (0f, 360f))),transform);
			go.GetComponent<Rigidbody> ().AddExplosionForce (500, goldPos, 10);
			go.GetComponent<Collider> ().enabled = false;
			DestroyGold (go);
		}
	}
}
