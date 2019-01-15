using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PlayerGuide : MonoBehaviour {
	//玩家刚体
	Rigidbody rig;
	//脚步碰撞器
	BoxCollider playerColl;
	[SerializeField]
	BoxCollider bodyColl;
	//玩家动画控制器
	Animator animator;
	//跳跃力
	[SerializeField]
	float force = 500;
	//重力
	[SerializeField]
	float gravity = 10;

	[SerializeField]
	GameObject hold;
	[SerializeField]
	GameObject continueHold;
	[SerializeField]
	GameObject finish;
	[SerializeField]
	GameObject stopHold;
	[SerializeField]
	GameObject holdFlip;
	[SerializeField]
	GameObject stopFlip;

	IEnumerator coco;

	//初始化
	void Start () {
		rig = GetComponent<Rigidbody>();
		playerColl = GetComponent<BoxCollider>();
		animator = GetComponent<Animator>();

		isRotate = false;

		hold.GetComponent<Text> ().DOColor (Color.white, 1);

		coco = RotateMid ();
	}

	int eulur = 0;
	bool guideFinish = false;
//	void Update()
//	{
//
//		if (!guideFinish) {
//
//			if (CheckGuiRaycastObjects()) return;
//			if (!startFlip) {
//				if (Input.GetMouseButton (0) || Input.GetKey (KeyCode.P)) {		
//					if (eulur > 20) {
//						stopHold.SetActive (true);
//					} else {
//						if (isRotate == false) {
//							GetPress ();
//						}
//						transform.Rotate (new Vector3 (1, 0, 0), -1);
//						eulur += 1;
//						continueHold.SetActive (false);
//					}
//				}
//									
//				if (Input.GetMouseButtonUp (0) || Input.GetKeyUp (KeyCode.P)) {
//					//if (transform.eulerAngles.x < 340 && transform.eulerAngles.x > 300) {
//					if (eulur > 20) {					
//						rig.constraints = RigidbodyConstraints.None;
//						Physics.gravity = new Vector3 (0, gravity, 0);
//						rig.AddForce (transform.up * force, ForceMode.Force);
//						transform.DOLocalRotate (new Vector3 (-transform.eulerAngles.x, 0, 0), 1.5f, RotateMode.LocalAxisAdd);
//						Invoke ("ReColl", 1);
//						animator.SetBool ("Storage", false);
//						animator.SetBool ("Jump", true);
//						isRotate = false;
//						stopHold.SetActive (false);
//
//						Invoke ("StartFlip", 0.2f);
//					} else {
//						continueHold.SetActive (true);
//					}
//				}							
//			}
//	
//			if (startFlip) {	
//				if (Input.GetMouseButtonDown (0)) {
//					Time.timeScale = 1;
//					holdFlip.SetActive (false);
//					continueHold.SetActive (false);
//				}
//				if (Input.GetMouseButton (0)) {
//					if (eulur <= 300) {
//						transform.Rotate (new Vector3 (-10, 0, 0));
//						eulur += 10;
//					} else {
//						Time.timeScale = 0;
//						//holdFlip.SetActive (false);
//						stopFlip.SetActive (true);
//					}
//				} if(Input.GetMouseButtonUp(0)){
//					if (eulur >= 300) {
//						Time.timeScale = 1;
//						continueHold.SetActive (false);
//						stopFlip.SetActive (false);
//					} else {						
//						Time.timeScale = 0;
//						continueHold.SetActive (true);
//					}
//				}
//			}
//
//		}	
//	}
	bool down = false;
	bool stopco = false;
	void Update(){
		if (!guideFinish) {
			if (Input.GetMouseButton (0)) {
				playerColl.enabled = false;
				down = true;
				animator.SetBool ("Storage", true);
				animator.SetBool ("Idle", false);
				hold.SetActive (false);
				continueHold.SetActive (false);
				if (isRotate == false) {
					StartCoroutine (coco);
				}
				if (stopco) {
					StartCoroutine (coco);
					stopco = false;
				}
			}	

			if (Input.GetMouseButtonUp (0)) {
				if (Mathf.Abs (power.fillAmount - 0.5f) < 0.1f) {
					HidePower (true);
					rig.constraints = RigidbodyConstraints.None;
					Physics.gravity = new Vector3 (0, gravity, 0);
					rig.AddForce (transform.up * force / 1.2f / 1.45f, ForceMode.Force);
					rig.AddForce (Vector3.up * power.fillAmount * force / 1.45f);

					transform.DOLocalRotate (new Vector3 (-transform.eulerAngles.x, 0, 0), 1f, RotateMode.LocalAxisAdd);

					Invoke ("ReColl", 0.1f);
					animator.SetBool ("Storage", false);
					animator.SetBool ("Jump2", true);

					isRotate = false;

					Camera.main.DOFieldOfView (70, 1).OnComplete (() => {
						Camera.main.DOFieldOfView (60, 1);
					});

					stopHold.SetActive (false);			

				} else {
					continueHold.SetActive (true);
					StopCoroutine (coco);
					stopco = true;
				}
			}


		}	
	}

	//蓄力旋转
	[SerializeField]
	Transform powerPos;
	bool isRotate;
	float MaxAngle;
	Image power;
	IEnumerator RotateMid()
	{		
		if (!isRotate)
		{
			isRotate = true;
			MaxAngle = 0;
			float totalAngle = 0;
			float speed = -0.8f;
			HidePower(false);
			power = powerUI.Find ("power").GetComponent<Image> ();
			powerUI.position = Camera.main.WorldToScreenPoint (powerPos.position);
			while (isRotate)
			{			
				MaxAngle += speed;
				totalAngle += Mathf.Abs(speed);

				if (Mathf.Abs(MaxAngle) > 60) {					
					speed *= -1;
					MaxAngle = 0;
				}			
				transform.Rotate(new Vector3(1, 0, 0), speed);

				if (MaxAngle > 0) {					
					power.fillAmount = 1-MaxAngle/60;
				} else if(MaxAngle < 0) {					
					power.fillAmount = MaxAngle/-60;
				}

				if (down) {
					if (Mathf.Abs (power.fillAmount - 0.5f) < 0.1f) {
						stopHold.SetActive (true);			
						yield break;
					}
				}
				yield return null;
			}
		}
	}

	[SerializeField]
	Transform powerUI;
	void HidePower(bool hide){
		Image bg = powerUI.Find ("powerBG").GetComponent<Image> ();
		Image power = powerUI.Find ("power").GetComponent<Image> ();
		Text max = powerUI.Find ("max").GetComponent<Text> ();
		if (hide) {
			bg.DOColor (Color.clear, 0.5f);
			power.DOColor (Color.clear, 0.5f);
			max.DOColor (Color.clear, 0.5f);
		} else {
			max.DOColor (new Color (1, 0, 31 / 255f), 0.2f);
			power.DOColor (Color.white, 0.2f);
			bg.DOColor (new Color(207/255f,207/255f,207/255f),0.2f);
		}
	}

	bool startFlip = false;
	void StartFlip(){
		startFlip = true;
		holdFlip.SetActive (true);
		Time.timeScale = 0;
	}

	void ReColl(){
		playerColl.enabled = true;
		bodyColl.enabled = true;
	}
		

	private void OnCollisionEnter(Collision coll)
	{
		if (coll.collider.name == "Plane") {
			FlyGold.Instance.GenerateGoldGuide (30, transform.position);
			rig.constraints = RigidbodyConstraints.FreezeAll;
			animator.SetBool("Idle", true);
			animator.SetBool("Jump", false);
			finish.SetActive (true);
			guideFinish = true;
		}
	}

	public void GoHome(){
		PlayerPrefs.SetInt ("GuideScene", 1);
		SceneManager.LoadScene ("Loading");
	}


	//蓄力旋转



	public EventSystem eventSystem;
	public GraphicRaycaster graphicRaycaster;
	bool CheckGuiRaycastObjects()
	{
		PointerEventData eventData = new PointerEventData(eventSystem);
		eventData.pressPosition = Input.mousePosition;
		eventData.position = Input.mousePosition;

		List<RaycastResult> list = new List<RaycastResult>();
		graphicRaycaster.Raycast(eventData, list);
		return list.Count > 0;
	}

}
