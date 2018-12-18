﻿using System.Collections;
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

	//初始化
	void Start () {
		rig = GetComponent<Rigidbody>();
		playerColl = GetComponent<BoxCollider>();
		animator = GetComponent<Animator>();

		isRotate = false;

		hold.GetComponent<Text> ().DOColor (Color.white, 1);
	}

	int eulur = 0;
	bool guideFinish = false;
	void Update()
	{

		if (!guideFinish) {

			if (CheckGuiRaycastObjects()) return;

			if (Input.GetMouseButton(0)||Input.GetKey(KeyCode.P)) {		
				if (transform.eulerAngles.x < 340 && transform.eulerAngles.x > 300) {
					stopHold.SetActive (true);
				} else {
					if (isRotate == false) {
						GetPress ();
					}
					transform.Rotate (new Vector3 (1, 0, 0), -1);
					eulur += 1;
					continueHold.SetActive (false);
				}
			}
									
			if (Input.GetMouseButtonUp(0)||Input.GetKeyUp(KeyCode.P)) {
				if (transform.eulerAngles.x < 340 && transform.eulerAngles.x > 300) {
					Debug.Log (eulur);
					rig.constraints = RigidbodyConstraints.None;
					Physics.gravity = new Vector3 (0, gravity, 0);
					rig.AddForce (transform.up * force, ForceMode.Force);
					transform.DOLocalRotate (new Vector3 (-transform.eulerAngles.x, 0, 0), 1.5f, RotateMode.LocalAxisAdd);
					Invoke ("ReColl", 1);
					animator.SetBool ("Storage", false);
					animator.SetBool ("Jump", true);
					isRotate = false;
					stopHold.SetActive (false);

					Invoke ("StartFlip", 0.2f);
				} else {
					continueHold.SetActive (true);
				}
			}							

	
			if (startFlip) {	
				if (Input.GetMouseButtonDown (0)) {
					Time.timeScale = 1;
					holdFlip.SetActive (false);
					continueHold.SetActive (false);
				}
				if (Input.GetMouseButton (0)) {
					if (eulur >= 340) {
						transform.Rotate (new Vector3 (-10, 0, 0));
						eulur += 10;
					} else {
						Time.timeScale = 0;
						holdFlip.SetActive (false);
						stopFlip.SetActive (true);
					}
				} if(Input.GetMouseButtonUp(0)){
					if (eulur >= 340) {
						Time.timeScale = 1;
						continueHold.SetActive (false);
						stopFlip.SetActive (false);
					} else {						
						Time.timeScale = 0;
						continueHold.SetActive (true);
					}
				}
			}

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
		SceneManager.LoadScene ("SampleScene");
	}


	//蓄力旋转
	bool isRotate;
	void GetPress()
	{		
		if (!isRotate)
		{
			isRotate = true;
			playerColl.enabled = false;
			bodyColl.enabled = false;
			animator.SetBool ("Storage", true);
			animator.SetBool ("Idle", false);
			hold.SetActive (false);
		}
	}

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
