using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Together;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    //玩家刚体
    Rigidbody rig;
    //脚步碰撞器
    BoxCollider playerColl;
	[SerializeField]
	BoxCollider bodyColl;
    //玩家动画控制器
    Animator animator;
	[HideInInspector]
	public GameObject city;   
	[SerializeField]
	Transform alarm;
	[SerializeField]
	GameObject daily;
	GameObject[] startsPos;
	//游戏内的UI
	GameObject[] gameUIs;
	GameObject[] moneyUIs;
	GameObject[] startUIs;
    //跳跃力
    [SerializeField]
    float force = 500;
	//小车撞击力
	[SerializeField]
	float carForce = 1000;
    //重力
    [SerializeField]
    float gravity = 10;
    //死亡触发面
    [SerializeField]
    GameObject deadPlane;
    [SerializeField]
    GameObject radar;
    [SerializeField]
    Transform ringManager;
	[SerializeField]
	GameObject boxGlove;
	[SerializeField]
	GameObject firstHole;
	[SerializeField]
	GameObject doubleSettle;
	[SerializeField]
	Transform powerPos;
	[SerializeField]
	Transform powerUI;
	string[] perfectWord;
    bool isCoroutining = false;
    //按住蓄力协程
    IEnumerator coroutine;
    //环列表
    List<string> rings;
	//金币箱子
	[SerializeField]
	GameObject goldBox;
	[SerializeField]
	LaunchArc launchArc;

	[HideInInspector]
	public Transform holeTarget;
	//获取环的位置
	public Vector3 ringPos;

	//关卡起始位置
	[HideInInspector]
	public Vector3 nextLevelPos;

	Vector3 currentColl;

	//城镇总的偏移
	Vector3 totalCityOffset;

	//城市移动的偏差
	[HideInInspector]
	public Vector3 cityOffset;

	[HideInInspector]
	public bool finishBackRing = false;

	public GameObject hitPs;

	Radar radarScript;

    //分数字典
	Dictionary<string, float> scoreDic;

	//记录旋转的度数
	int eulurX = 0;
	int targetEulur = 150;
	//旋转速度
	int eulurSpeed = 2;
	//
	int dailyTimes = 0;

	[HideInInspector]
	public GameObject tempText;
	[HideInInspector]
	public string tempTextStr;

	Transform playerParent;
    //0:待机状态
    //1:蓄力状态
    //2:跳跃状态
    //3:死亡状态
    //4:获胜状态
    //5:结算状态
    //6:进入黑洞状态
    //7:进入新关卡
    //8:转向环心
	//9:被撞
	//10:蓄力过久
    [HideInInspector]
    public int GameState = 0;

	int floorNumber;

	[HideInInspector]
	public bool Starting = false;

    static PlayerController instance;
    public static PlayerController Instance{
        get { return instance; }
    }   
    private void Awake()
    {
        instance = this;
    }

    //初始化
    void Start () {		
		if (PlayerPrefs.GetInt ("GuideScene", 0) == 0) {
			SceneManager.LoadScene ("GuideScene");
		}

        rig = GetComponent<Rigidbody>();

        playerColl = GetComponent<BoxCollider>();

		animator = GetComponent<Animator>();
		radarScript = radar.GetComponent<Radar>();		      

        rings = new List<string>();
		scoreDic = new Dictionary<string, float>();	
		cityOffset = new Vector3 ();
		//playerParent = transform.parent;
		perfectWord = new string[]{ "GREAT", "GOOD", "PERFECT", "PRETTY" };

		moneyUIs = GameObject.FindGameObjectsWithTag ("money");
		gameUIs = GameObject.FindGameObjectsWithTag ("GameUI");
		startUIs = GameObject.FindGameObjectsWithTag ("StartUI");

		HideGameUI (true);

		coroutine = RotateMid();
		totalCityOffset = Vector3.zero;

		InitScoreDic();
		InitRingMatDic ();

		floorNumber = 0;
		isRotate = false;
		dailyTimes = 0;
	}

	//tap开始游戏,配合开始按钮使用
	public void GameStart(){		
		Physics.gravity = new Vector3(0, gravity, 0);
		playerColl.enabled = false;
		bodyColl.enabled = false;
		GameState = 2;
		animator.SetBool ("Storage", true);
		animator.SetBool ("Idle", false);
		StartCoroutine (CheckRotation ());
	}

	//重置关卡
	[SerializeField]
	Transform initPos;
	[SerializeField]
	Transform initPrePos;
	public void ChangeLevel(){
		int curMapIndex = PlayerPrefs.GetInt ("CurMap", 0);
		PlayerPrefs.SetInt ("Level"+curMapIndex, PlayerPrefs.GetInt ("Level"+curMapIndex, 1) + 1);		

		SaveManager.Instance.ClearPosList ();
		radarScript.levelPos.Clear ();	
		SaveManager.Instance.SetVector3 ("TotalCityOffset", Vector3.zero);
		Camera.main.transform.position += initPos.position - initPrePos.position;
		Physics.gravity = new Vector3(0, gravity, 0);
		GameState = 2;
		animator.SetBool ("Idle", false);
		animator.SetBool ("Storage", true);
		animator.SetBool ("Storage", false);
		SetJumpBool (true);
		rig.constraints = RigidbodyConstraints.None;
		transform.position = initPos.position;
	}
		

	//重新开始关卡
	//当前出生点
	public Vector3 ReGamePos = Vector3.zero;
	public void ReGame(){
		if (GameState != 9 && GameState != 3) {
			return;
		}
		//重置位置
		transform.position = ReGamePos;

		//重置状态
		GameState = 0;
		rig.constraints = RigidbodyConstraints.FreezeAll;
		transform.eulerAngles = Vector3.zero;

		//清除环
		ClearRing ();
		//清除黑洞
		if(holeTarget){
			Destroy (holeTarget.gameObject);
		}
		//重新扫描,清空已跳跃的数量
		radarScript.jumpCount = 0;
		radarScript.levelPos.Clear ();
		RadarScan();

		//重置动画
		animator.SetBool("Idle", true);
		SetJumpBool (false);
		animator.SetBool("Storage", false);
		animator.SetBool("Dead", false);

		//删除拳套
		if (boxGloveTrans)
			Destroy (boxGloveTrans.gameObject);
		if (tempText) {
			Destroy (tempText);
		}
	}

	public void ReGameClick(){	
		if (isRotate) {
			return;
		}	
		//重置位置
		transform.position = ReGamePos;

		//重置状态
		GameState = 0;
		rig.constraints = RigidbodyConstraints.FreezeAll;
		transform.eulerAngles = Vector3.zero;

		//清除环
		ClearRing ();
		//清除黑洞
		if(holeTarget){
			Destroy (holeTarget.gameObject);
		}
		//重新扫描,清空已跳跃的数量
		radarScript.jumpCount = 0;
		radarScript.levelPos.Clear ();
		RadarScan();

		//重置动画
		animator.SetBool("Idle", true);
		SetJumpBool (false);
		animator.SetBool("Storage", false);
		animator.SetBool("Dead", false);

		//删除拳套
		if (boxGloveTrans)
			Destroy (boxGloveTrans.gameObject);
		if (tempText) {
			Destroy (tempText);
		}
	}
		
	public GameObject ResetLevel;
	public void OnReGameBtn(){
		ResetLevel.SetActive (true);
	}

	public void OnReGameBackBtn(){
		ResetLevel.SetActive (false);
	}

	public GameStart gameStartScript;
	public void OnFreeReGameBtn(){
		if (TGSDK.CouldShowAd (TGSDKManager.resetLevelID)) {
			TGSDK.ShowAd (TGSDKManager.resetLevelID);
			TGSDK.AdCloseCallback = (string obj) => {
				gameStartScript.ChangeLevel();
			};
		} else {
			TipPop.GenerateTip ("no ads", 0.5f);
		}
	}

	public void OnDiamondBtn(){
		if (Diamond.Instance.UseDiamond (25)) {
			Diamond.Instance.UpdateDiamond ();
			gameStartScript.ChangeLevel();
		}
	}

	//检查tap后的主角角度,用于开始按钮
	IEnumerator CheckRotation(){		
		while (true) {
			transform.eulerAngles += new Vector3 (-Time.deltaTime*7, 0, 0);
			if (transform.eulerAngles.x < 350 && transform.eulerAngles.x > 300) {		
				rig.constraints = RigidbodyConstraints.None;
				animator.SetBool ("Storage", false);
				SetJumpBool (true);
				rig.AddForce (transform.up * force, ForceMode.Force);
				transform.DOLocalRotate (new Vector3 (-transform.eulerAngles.x, 0, 0), 1.5f, RotateMode.LocalAxisAdd);
				Invoke ("ReColl", 1);
				yield break;
			}
			yield return null;
		}
	}
		
	void SetJumpBool(bool boo){
		int index = PlayerPrefs.GetInt ("curSelect", 0);
		for (int i = 0; i < 5; i++) {
			if (i == index) {
				animator.SetBool ("Jump" + i, boo);
			} else {
				animator.SetBool ("Jump" + i, false);
			}
		}
	}

	//随机获得一个出生点
	//该位置没有进行过”偏移“和”深度改变“
	GameObject[] startsPosArray;
	Vector3 InitPlayerPos(){
		startsPos = GameObject.FindGameObjectsWithTag ("start");
		startsPosArray = new GameObject[startsPos.Length];
		for (int i = 0; i < startsPos.Length; i++) {
			int index = int.Parse (startsPos [i].name.Split (new string[] { "art" }, System.StringSplitOptions.None) [1]);
			startsPosArray [index - 1] = startsPos [i];
		}

		int curLevel = 1;
		int curMapIndex = PlayerPrefs.GetInt ("CurMap", 0);
		if (floorNumber == 1) {			
			curLevel = PlayerPrefs.GetInt ("Level"+curMapIndex, 1);
		} else {
			curLevel = PlayerPrefs.GetInt ("Level"+curMapIndex, 1) + 1;
		}
		Vector3 startPos;
		int maxLevel = 0;
		if (curMapIndex == 0) {
			maxLevel = 40;
		} else if (curMapIndex == 1) {
			maxLevel = 24;
		} else if (curMapIndex == 2) {
			maxLevel = 10;
		}
		if (curLevel <= maxLevel) {
			startPos = startsPosArray [curLevel - 1].transform.position;
			InitRadarRings (startsPosArray [curLevel - 1].transform);
			targetStartPos = startsPosArray [curLevel - 1].transform;
		} else {
			int nextLevel = PlayerPrefs.GetInt ("Level" + curMapIndex + curLevel, 0);
			if(nextLevel==0){
				nextLevel = Random.Range (0, maxLevel);
			}
			PlayerPrefs.SetInt ("Level" + curMapIndex + curLevel, nextLevel);
			startPos = startsPosArray [nextLevel].transform.position;
			InitRadarRings (startsPosArray [nextLevel].transform);
			targetStartPos = startsPosArray [nextLevel].transform;
		}
		return startPos;
	}

	void InitRadarRings(Transform target){
		int count = target.childCount;
		radarScript.curLevelRings = new Vector3[count];
		radarScript.startPos = target;
		for(int i=0;i<count;i++){
			radarScript.curLevelRings [i] = target.GetChild (i).position - target.position;
		}
	}

	public int flipNumber=0;
    void Update()
	{
		
		if (Input.touchCount == 3 && Input.GetTouch (1).phase == TouchPhase.Moved && Input.GetTouch (2).phase == TouchPhase.Moved) {
			Diamond.Instance.GetDiamond (777);
			MoneyManager.Instance.UpdateDiamond ();
		}
		if (Starting) {
			//待机状态
			//按住P开始蓄力，进入蓄力状态，碰撞取消，播放蓄力动画
			if (GameState == 0&&finishBackRing) {    				
				if (Input.GetMouseButton (0) && !CheckGuiRaycastObjects ()) {
				//if (Input.GetKeyDown (KeyCode.P)) {
					GameState = 1;
					playerColl.enabled = false;
					//bodyColl.enabled = false;
					animator.SetBool ("Storage", true);
					animator.SetBool ("Idle", false);
					//int speedLevel = PlayerPrefs.GetInt ("speedLevel", 1);
					//eulurSpeed = 2;
				}					

			}
			
        //蓄力状态
        //松开跳跃，蓄力过久死亡
        	else if (GameState == 1) {
               
				//开始改变角度
				if (isRotate == false) {
					StartCoroutine (RotateMid());
				}

				//松开P停止蓄力，接触角色限制，改变重力，往头朝向发射，旋转一圈，进入跳跃状态，1秒后恢复碰撞，开始跳跃动画，停止旋转，摄像机扩大视野范围，然后缩小视野范围，取消警告
				if (Input.GetMouseButtonUp (0) ) {
					finishBackRing = false;
					HidePower(true);
				//if (Input.GetKeyUp (KeyCode.P)) {
					eulurX = 0;
					targetEulur = 150;
					// Debug.Log(transform.eulerAngles.x - 360);
					rig.constraints = RigidbodyConstraints.None;
					Physics.gravity = new Vector3 (0, gravity, 0);
					rig.AddForce (transform.up * force/1.2f/1.45f, ForceMode.Force);
					rig.AddForce (Vector3.up * power.fillAmount * force/1.45f);

					//print (power.fillAmount);
					//rig.AddForce (Vector3.up * (1/-transform.up.z) * force, ForceMode.Force);
					//rig.AddForce (new Vector3 (-transform.up.x, 0, -transform.up.z) * force/1.8f, ForceMode.Force);
					//rig.AddForce (Vector3.forward * force/3, ForceMode.Force);
					flipNumber = PlayerPrefs.GetInt ("curLevel" + PlayerPrefs.GetInt ("curSelect", 0), 1)-1;	
					StartCoroutine(GenerateGoldByFlip(flipNumber,1f));
					transform.DOLocalRotate (new Vector3 (-transform.eulerAngles.x - 360 * flipNumber, 0, 0), 1f, RotateMode.LocalAxisAdd);

					GameState = 2;
					Invoke ("ReColl", 0.1f);
					animator.SetBool ("Storage", false);
					SetJumpBool (true);

					isRotate = false;
					//StopCoroutine (coroutine);

					Camera.main.DOFieldOfView (70, 1).OnComplete (() => {
						Camera.main.DOFieldOfView (60, 1);
					});

					alarm.gameObject.SetActive (false);
					isAlarming = false;
				}

			}
        //跳跃状态
        else if (GameState == 2) {	
				if (exitHole) {

				}
			}
        //死亡状态
        else if (GameState == 3) {
       		
			}
        //获胜状态
        else if (GameState == 4) {
           
			}
        //结算状态
        else if (GameState == 5) {
        
			}
        //进入黑洞
        else if (GameState == 6) {

			}
        //抵达新关卡
        else if (GameState == 7) {
           
			}
        //转向环心
        else if (GameState == 8) {

			}
		}

		//按下D删除游戏数据
		if (Input.GetKeyDown (KeyCode.D)) {
			PlayerPrefs.DeleteAll ();
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			ReGameClick ();
		}
    }

	//累积获得的金币
	int curGold = 0;
	IEnumerator GenerateGoldByFlip(int flipnumber,float time){
		curGold = 0;
		float timeDu = time /(flipnumber+1);
		int coinLevel = PlayerPrefs.GetInt ("coinLevel", 1);
		int curAction = (PlayerPrefs.GetInt ("curSelect", 0))*2+1;
		int getGold = ((int)(77 * (Mathf.Pow (1.05f, coinLevel)))) * curAction;
		yield return new WaitForSeconds (timeDu/5); 
		for (int i = 0; i < flipnumber+1; i++) {
			if (tempText) {
				Destroy (tempText);
			}
			getGold = (int)(getGold*(i*0.2f + 1));
			TipPop.GenerateTipStay ("$"+getGold, 0.5f, Color.white);
			FlyGold.Instance.GenerateGoldNoColl (20, transform.position);
			curGold = getGold;
			yield return new WaitForSeconds (timeDu-(flipnumber-i)*timeDu/10);
		}
		//curGold = getGold;
	}

	//恢复玩家碰撞
    void ReColl(){
        playerColl.enabled = true;
		//bodyColl.enabled = true;
    }
		

	//延迟进入主界面
	void GameOver(float delay){
		Invoke ("GameOver", delay);
	}

	//进入主界面
	public void GameOver(){
        ResetGame.Instance.OnResetBtn();
    }

	Transform targetStartPos;
    //碰撞处理
    //跳跃状态才能碰撞
    //撞到一般物体加少量分数,若超过目标点则生成新的目标点，进入待机状态，生成分数
    //身体先发生碰撞则判定死亡
    private void OnCollisionEnter(Collision coll)
    {
		//离开黑洞后状态为进入新关卡，此时若碰到一般建筑则...
		if (GameState == 7 && coll.collider.tag == "Untagged") {
			transform.position = targetStartPos.position;
			rig.constraints = RigidbodyConstraints.FreezeAll;
			//状态变为待机，播放待机动画
			GameState = 0;
			animator.SetBool("Idle", true);
			SetJumpBool (false);
			//如果是进入新关卡则增加关卡数并刷新关卡UI
			if(Level.Instance.slider.value == 1){
				int curMapIndex = PlayerPrefs.GetInt ("CurMap", 0);
				PlayerPrefs.SetInt ("Level"+curMapIndex, PlayerPrefs.GetInt ("Level"+curMapIndex, 1) + 1);
				Level.Instance.UpdateLevel ();

				if(PlayerPrefs.GetInt ("Level"+curMapIndex, 1)>=10&&PlayerPrefs.GetInt ("TurnHomeFinish", 0)==0){
					TurntableGuide.Instance.StartGuide ();
				}
				if(PlayerPrefs.GetInt ("Level"+curMapIndex, 1)>=40&&PlayerPrefs.GetInt ("MapHomeFinish", 0)==0){
					MapGuide.Instance.StartGuide ();
				}
				if (PlayerPrefs.GetInt ("SkillFinish", 0)==0) {
					
					int gold = PlayerPrefs.GetInt ("Gold", 0);
					int needGold = PlayerPrefs.GetInt ("SkillGold0", 281);
					if (needGold == 281) {
						if (gold >= needGold) {
							SkillGuide.Instance.StartGuide ();
						}
					} else {
						PlayerPrefs.SetInt ("SkillFinish", 1);
					}
				}
			}
			//生成新环
			RadarScan();
			//显示关卡UI
			HideGameUI (false);
			HideMoneyUI (false);

			if (tempText) {
				TipPop.GenerateTip ("X5", 0.5f,Color.white);
				Destroy (tempText,0.5f);
				PlayerPrefs.SetInt ("LevelPassGold", curGold*5);
				Gold.Instance.GetGold (curGold*5);
				MoneyManager.Instance.UpdateGold ();
				couldShowDoubl = true;
				TipPop.GenerateTipPerfect ("level up", 1f, Color.yellow);
			}
				
		}

		//如果是跳跃状态
        if (GameState == 2)
        {
			//如果碰到了一般建筑
            if (coll.collider.tag == "Untagged")
            {
				float playerEulerX = transform.eulerAngles.x;
				//if((eulurX%360)>=295&&(eulurX%360)<=360){
				//if ((playerEulerX > 325 && playerEulerX < 360) || (playerEulerX > 0 && playerEulerX < 50)) {
					//获得当前碰撞的点
					currentColl = coll.contacts [0].point;
					//检查射线下是否有环
					CheckRingByRay();
					//等待一段时间根据环生成分数，生成新的环
					Invoke("ScoreGenerate", 0.05f);
					Invoke("GenerateNewRing", 0.1f);
					//一段时间后锁定主角
					rig.constraints = RigidbodyConstraints.FreezeAll;

				//} else {
				//	GameOverByBoxglove (currentColl);
				//	TipPop.GenerateTip ("MISS", 0.5f);	
					//GameOverByBoxglove (transform.position + new Vector3 (0, -5, 0));
				//}

            }

        }

		//如果蓄力过度则被拳头打飞
		if (GameState == 10) {
			//GameOverByBoxglove (transform.position + new Vector3 (0, -5, 0));
		}

		//如果被车撞了，状态变为被车撞，解除主角限制，往被撞的方向施加力，给予主角随机旋转，三秒后重开游戏
		if (coll.collider.tag == "car"&&GameState!=9) {
			isRotate = false;
			HidePower (true);
			GameState = 9;
			rig.constraints = RigidbodyConstraints.None;
			Vector3 carDirection= (transform.position-coll.transform.position).normalized;
			rig.AddForce((carDirection + transform.up) * carForce, ForceMode.Force);
			Instantiate (hitPs, transform.position+new Vector3(0,3,0), Quaternion.identity);
			transform.DOLocalRotate(new Vector3(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)), 1.5f, RotateMode.LocalAxisAdd);
			Invoke("ReGame",3);
			PlayerPrefs.SetInt ("CarHit", PlayerPrefs.GetInt ("CarHit", 0) + 1);
			if (PlayerPrefs.GetInt ("vibration", 1)==1)
				MultiHaptic.HapticHeavy ();
			if (tempText) {
				Destroy (tempText);
			}
			dailyTimes++;
			if (dailyTimes >= 3) {
				Invoke ("ShowDaily", 3);
			}
		}

    }

	void GenerateNewRing(){
		//没死的话，清除环，生成环，播放待机动画    
		if (GameState != 3) {			
			ClearRing ();
			RadarScan ();
			animator.SetBool("Idle", true);
			SetJumpBool (false);
		}
	}

	//临时存储拳头对象
	Transform boxGloveTrans;
	//被拳头打死
	void GameOverByBoxglove(Vector3 golvePos){
		//在死亡状态或待机状态不作处理
		if (GameState == 3||GameState == 0) {
			return;
		}
		isRotate = false;
		HidePower (true);
		//状态变为死亡状态,解放主角，被打方向施加力，随机旋转，三秒后重置游戏
		GameState = 3;
		rig.constraints = RigidbodyConstraints.None;
		Vector3 carDirection= (transform.position-golvePos).normalized;
		rig.AddForce((carDirection + transform.up) * carForce, ForceMode.Force);
		Instantiate (hitPs, transform.position+new Vector3(0,3,0), Quaternion.identity);
		//rig.AddForce(Vector3.up* carForce*1.3f, ForceMode.Force);
		transform.DOLocalRotate(new Vector3(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)), 1.5f, RotateMode.WorldAxisAdd);
		Invoke("ReGame",3);
		//在主角位置生成拳套，面向主角出拳
		boxGloveTrans = Instantiate (boxGlove, transform.position, Quaternion.identity).transform;
		boxGloveTrans.up = carDirection;
		//boxGloveTrans.up = Vector3.up;
		boxGloveTrans.DOMove (transform.position+Vector3.up*3, 0.3f, false);
		//取消警告
		alarm.gameObject.SetActive (false);

		PlayerPrefs.SetInt ("GloveHit", PlayerPrefs.GetInt ("GloveHit", 0) + 1);

		if (tempText) {
			Destroy (tempText);
		}

		dailyTimes++;
		if (dailyTimes >= 3) {
			Invoke ("ShowDaily", 3);
		}
		if (PlayerPrefs.GetInt ("vibration", 1) == 1)
			MultiHaptic.HapticHeavy ();
	}

	void ShowDaily(){		
		daily.SetActive (true);
		dailyTimes = 0;
	}

	//清空环
    void ClearRing(){
        for (int i = ringManager.childCount - 1; i >= 0;i--){
            Destroy(ringManager.GetChild(i).gameObject);
        }
    }

    //触发处理
    //触发相应的环则增加相应的分数
    //触发黑洞生成新的关卡
    //触发死亡面进入死亡状态
    private void OnTriggerEnter(Collider other)
    {
		//进入黑洞则往黑洞中间施加力，隐藏UI
		if (other.tag == "hole") {
			StartCoroutine (AddForceInHole ());
			HideGameUI (true);
			HideMoneyUI (true);
		}if (other.name == "point") {

			other.transform.DOScale (new Vector3 (2, 2, 2), 1f);
			other.GetComponent<SpriteRenderer> ().DOColor (Color.clear, 1f).OnComplete(()=>{
				Destroy(other);
			});
		}
    }

	//往黑洞中间施加力，之后改为被黑洞吸引
	IEnumerator AddForceInHole(){
//		int i = 3;
//		while (i <= 0) {
//			i--;
//			rig.AddForce ((holeTarget.position - transform.position)*50);
//			yield return new WaitForSeconds (0.2f);
//		}
		if (holeTarget) {
			transform.DOMove (holeTarget.position, 0.2f, false);
		}
		
		yield return null;
	}		

	[HideInInspector]
	//记录进入黑洞后的玩家位置
	public Vector3 transformPre = Vector3.zero;
	bool exitHole = false;
//	void OnTriggerExit(Collider other)
//	{
//		//如果离开了黑洞
//		if (other.tag == "hole")
//		{		
//			GameObject[] inholes = GameObject.FindGameObjectsWithTag ("Player");
//			foreach (GameObject go in inholes) {
//				go.layer = 0;
//			}
//
//			exitHole = true;
//
//			//删除存在的黑洞
//			if (firstHole) {
//				firstHole.SetActive(false);
//			}
//			if(holeTarget){
//				Destroy(holeTarget.Find("Hole").gameObject);
//			}
//
//			//记录下降层数
//			//radarScript.floorNumber = PlayerPrefs.GetInt ("Floor", 0);
//			//PlayerPrefs.SetInt ("Floor", floorNumber);
//			floorNumber++;
//
//			//锁定主角的X和Z坐标
//			rig.constraints = RigidbodyConstraints.FreezePositionX;
//			rig.constraints = RigidbodyConstraints.FreezePositionZ;
//
//			//反向移动城市
//			//穿过黑洞后城镇往下移动100米，获得一个随机出生点
//			city.transform.position += new Vector3(0, -100, 0);
//			nextLevelPos = InitPlayerPos ();
//			//进入第一层时,历史记录为空则随机生成，否则使用历史记录
//			//第二层开始一直取随机位置
//			if (floorNumber == 1) {
//				Vector3 preCityoffset = SaveManager.Instance.GetVector3 ("TotalCityOffset");
//				if (preCityoffset == Vector3.zero) {
//					cityOffset = transform.position - new Vector3 (nextLevelPos.x, 0, nextLevelPos.z);
//				} else {
//					cityOffset = new Vector3 (preCityoffset.x, 0, preCityoffset.z);
//				}
//				totalCityOffset += cityOffset;
//			} else if(floorNumber>1) {				
//				SaveManager.Instance.ClearPosList ();
//				radarScript.levelPos.Clear ();
//				cityOffset = transform.position - new Vector3 (nextLevelPos.x, 0, nextLevelPos.z);
//				totalCityOffset += transformPre - nextLevelPos;
//			}
//			//每一层记录城市总位移
//			SaveManager.Instance.SetVector3 ("TotalCityOffset", totalCityOffset);
//
//			Vector3 cityPos = city.transform.position;
//			city.transform.DOLocalMoveX (cityPos.x + cityOffset.x, 0.1f, false);
//			city.transform.DOLocalMoveZ (cityPos.z + cityOffset.z, 0.1f, false).OnComplete(()=>{
//				//纠正人物方向
//				transform.DORotate (new Vector3 (0, 180, 0), 1.5f, RotateMode.Fast);
//			});	
//			//新关卡生成盒子
//			Instantiate (goldBox, nextLevelPos+new Vector3(cityOffset.x,0,cityOffset.z),Quaternion.identity);
//			//记录本次离开黑洞后的玩家位置，供下次使用
//			transformPre = transform.position;
//		}
//	}
	void OnTriggerExit(Collider other)
	{
		//如果离开了黑洞
		if (other.tag == "hole")
		{		
			GameObject[] inholes = GameObject.FindGameObjectsWithTag ("Player");
			foreach (GameObject go in inholes) {
				go.layer = 0;
			}

			HideStartUI (true);

			exitHole = true;

			//删除存在的黑洞
			if (firstHole) {
				firstHole.SetActive(false);
			}
			if(holeTarget){
				Destroy(holeTarget.Find("Hole").gameObject);
			}

			floorNumber++;

			//锁定主角的X和Z坐标
			rig.constraints = RigidbodyConstraints.FreezePositionX;
			rig.constraints = RigidbodyConstraints.FreezePositionZ;

			//反向移动城市
			//穿过黑洞后城镇往下移动100米，获得一个当前关卡出生点
			nextLevelPos = InitPlayerPos ();
			city.transform.position += new Vector3(0, -100, 0);
			//进入第一层时,历史记录为空则随机生成，否则使用历史记录
			//第二层开始一直取随机位置
			if (floorNumber == 1) {
				SaveManager.Instance.ClearPosList ();
				radarScript.levelPos.Clear ();
				Vector3 preCityoffset = SaveManager.Instance.GetVector3 ("TotalCityOffset");
				if (preCityoffset == Vector3.zero) {
					cityOffset = transform.position - new Vector3 (nextLevelPos.x, 0, nextLevelPos.z);
				} else {
					cityOffset = new Vector3 (preCityoffset.x, 0, preCityoffset.z);
				}
				totalCityOffset += cityOffset;
			} else if(floorNumber>1) {				
				SaveManager.Instance.ClearPosList ();
				radarScript.levelPos.Clear ();
				cityOffset = transform.position - new Vector3 (nextLevelPos.x, 0, nextLevelPos.z);
				totalCityOffset += transformPre - nextLevelPos;
			}
			//每一层记录城市总位移
			SaveManager.Instance.SetVector3 ("TotalCityOffset", totalCityOffset);

			Vector3 cityPos = city.transform.position;
			city.transform.DOLocalMoveX (cityPos.x + cityOffset.x, 0.1f, false);
			city.transform.DOLocalMoveZ (cityPos.z + cityOffset.z, 0.1f, false).OnComplete(()=>{
				//纠正人物方向
				transform.DORotate (new Vector3 (0, 180, 0), 1.5f, RotateMode.Fast);
			});	
			//新关卡生成盒子
			Instantiate (goldBox, nextLevelPos+new Vector3(cityOffset.x,0,cityOffset.z),Quaternion.identity);
			//记录本次离开黑洞后的玩家位置，供下次使用
			transformPre = transform.position;
		}
	}

	Dictionary<string,Transform> currentRing = new Dictionary<string, Transform>();
	//通过射线检测主角下方处是否含有环，有则添加进环数组，环的碰撞取消
    void CheckRingByRay(){
		Ray ray = new Ray(transform.position+new Vector3(0,5,0), Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach(RaycastHit hit in hits){
            string hitName = hit.collider.name;
			if (hitName.StartsWith ("Road")) {
				PlayerPrefs.SetInt ("Road", PlayerPrefs.GetInt ("Road", 0)+1);
			}
			if (hitName.StartsWith ("Houes")||hitName.StartsWith ("Garage")||hitName.StartsWith ("Gas_Station")||hitName.StartsWith ("Food_bar")) {
				PlayerPrefs.SetInt ("Roof", PlayerPrefs.GetInt ("Roof", 0)+1);
			}
			if (hitName.StartsWith ("Car")) {
				PlayerPrefs.SetInt ("CarUp", PlayerPrefs.GetInt ("CarUp", 0)+1);
			}
            if(hitName.StartsWith("ring")){
                rings.Add(hitName);
				if (currentRing.ContainsKey (hitName)) {
					currentRing.Remove (hitName);
					currentRing.Add (hitName, hit.transform);
				} else {
					currentRing.Add (hitName, hit.transform);
				}
                hit.collider.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }		


    //生成分数,还需要生成文字perfect提示
    void ScoreGenerate(){	
		if (!scoreGenerating) {
			scoreGenerating = true;
			Invoke ("ResetScoreGenerate", 1);
			//如果没有踩中环,弹出MISS并拳头打飞
			string ringname = CheckRing ();
			if (ringname == "normal" && GameState != 0) {				
				//GameState = 0;
				//rings.Clear ();
				//if (tempText) {
				//	TipPop.GenerateTip ("X1", 0.5f,Color.yellow);					
				//	int coinLevel = PlayerPrefs.GetInt ("coinLevel", 1);			
				//	Gold.Instance.GetGold ((int)(77*(Mathf.Pow(1.05f,coinLevel))));
				//	MoneyManager.Instance.UpdateGold ();
				Destroy (tempText,0.5f);
				if (PlayerPrefs.GetInt ("vibration", 1) == 1)
					MultiHaptic.HapticMedium ();
				//TipPop.GenerateTip ("MISS", 0.5f);
				GameOverByBoxglove (currentColl);
				FlyGold.Instance.GenerateGoldNoColl (30, transform.position+Vector3.up*2);
				//TipPop.GenerateTip ("-$"+curGold, 0.5f,Color.yellow);
				//}
				return;		
			} else {
				//踩中环的时候状态变为待机状态,生成对应的分数，删除踩中的环
				GameState = 0;

				Transform ringTrans = Instantiate (currentRing [ringname + "(Clone)"].gameObject).transform;
				ringTrans.DOScale (ringTrans.localScale * 3, 0.5f);
				MeshRenderer ringMesh = ringTrans.GetComponent<MeshRenderer> ();
				ringMesh.material = ringMarDic [ringname];
				ringMesh.material.DOColor (Color.clear, 0.5f).OnComplete (() => {
					Destroy (ringTrans.gameObject);
				});

				rings.Clear ();
				if (PlayerPrefs.GetInt ("vibration", 1) == 1)
					MultiHaptic.HapticMedium ();
				if (tempText) {
					TipPop.GenerateTip ("X"+scoreDic [ringname], 0.5f,Color.white);
					Destroy (tempText,0.5f);
					//int coinLevel = PlayerPrefs.GetInt ("coinLevel", 1);	
					Gold.Instance.GetGold ((int)(curGold*scoreDic [ringname]));
					MoneyManager.Instance.UpdateGold ();
				}
				TipPop.GenerateTipPerfect (perfectWord [Random.Range(0, perfectWord.Length)], 1f, Color.yellow);
			}
		}
    }

	bool scoreGenerating = false;
	void ResetScoreGenerate(){
		scoreGenerating = false;
	}

    //判断踩中的环
    string CheckRing(){
		if(rings.Contains("ring1(Clone)")||rings.Contains("ring1_mid(Clone)")||rings.Contains("ring1_small(Clone)")){
            return "ring1";
		}else if(rings.Contains("ring2(Clone)")||rings.Contains("ring2_mid(Clone)")||rings.Contains("ring2_small(Clone)")){
            return "ring2";
		}else if (rings.Contains("ring3(Clone)")||rings.Contains("ring3_mid(Clone)")||rings.Contains("ring3_small(Clone)")){
            return "ring3";
        }else{
            return "normal";
        }
    }

    //蓄力旋转
	[HideInInspector]
	public bool isRotate;
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
			//powerUI.gameObject.SetActive (true);
			HidePower(false);
			power = powerUI.Find ("power").GetComponent<Image> ();
			powerUI.position = Camera.main.WorldToScreenPoint (powerPos.position);
			while (isRotate)
			{			
				MaxAngle += speed;
				totalAngle += Mathf.Abs(speed);
				if (totalAngle > 400&&!isAlarming) {
					StartCoroutine (AlarmStorage ());
				}
				if(totalAngle>590&&isAlarming){
					GameOverByBoxglove (transform.position + new Vector3 (0, -5, 0));
					yield break;
				}
				if (Mathf.Abs(MaxAngle) > 60) {					
					speed *= -1;
					MaxAngle = 0;
				}			
				transform.Rotate(new Vector3(1, 0, 0), speed);


				if (MaxAngle > 0) {
				//	launchArc.angle = 30 + MaxAngle;
					power.fillAmount = 1-MaxAngle/60;
				} else if(MaxAngle < 0) {
				//	launchArc.angle = 90 + MaxAngle;
					power.fillAmount = MaxAngle/-60;
				}
				//launchArc.RenderArc ();
				yield return null;
            }
        }
    }

//	void FixedUpdate(){
//
//	}

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
		

    //初始化环倍数
    void InitScoreDic(){
        scoreDic.Add("ring1", 2);
        scoreDic.Add("ring2", 1.5f);
        scoreDic.Add("ring3", 1.2f);
        scoreDic.Add("normal", 1);
    }


	Dictionary<string,Material> ringMarDic = new Dictionary<string, Material>();
	public Material ring1Mat;
	public Material ring2Mat;
	public Material ring3Mat;
	void InitRingMatDic(){
		ringMarDic.Add("ring1", ring1Mat);
		ringMarDic.Add("ring2", ring2Mat);
		ringMarDic.Add("ring3", ring3Mat);
	}

	//扫描是否有可以生成环的位置
    void RadarScan()
    {
        radar.SetActive(true);
        Invoke("ScanOver", 0.2f);
    }
    void ScanOver(){
        radar.SetActive(false);
    }

	//触发警报
	bool isAlarming  = false;
	IEnumerator AlarmStorage(){	
		if (!isAlarming) {
			isAlarming = true;
			alarm.gameObject.SetActive (true);
			alarm.forward = Camera.main.transform.position - transform.position;
			SpriteRenderer sprite = alarm.GetComponent<SpriteRenderer> ();
			float alpha = 1;
			bool changeAlpha = false;
			while (true) {
				if (changeAlpha) {
					alpha -= Time.deltaTime*10;
					if (alpha < 0.01f) {
						changeAlpha = !changeAlpha;
					}
				} else {
					alpha += Time.deltaTime*10;
					if (alpha > 0.99f) {
						changeAlpha = !changeAlpha;
					}
				}
				sprite.color = new Color (1, 1, 1, alpha);
				yield return null;
			}

		}
	}

	//隐藏tag为GameUI的对象
	void HideGameUI(bool hide){
		foreach (GameObject go in gameUIs) {
			go.SetActive (!hide);
		}
	}

	void HideMoneyUI(bool hide){		
		foreach (GameObject go in moneyUIs) {
			go.SetActive (!hide);
		}
	}

	void HideStartUI(bool hide){		
		foreach (GameObject go in startUIs) {
			go.SetActive (!hide);
		}
	}

	bool couldShowDoubl = false;
	void ShowDouble(){
		if (couldShowDoubl) {
			Time.timeScale = 0;
			doubleSettle.SetActive (true);
			couldShowDoubl = false;
		}
	}
	public void ShowDoubleD(float time){
		Invoke ("ShowDouble", time);		
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
