using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour {
    //玩家刚体
    Rigidbody rig;
    //脚步碰撞器
    BoxCollider playerColl;
	[SerializeField]
	BoxCollider bodyColl;
    //玩家动画控制器
    Animator animator;
    [SerializeField]
    GameObject city;   
	[SerializeField]
	Transform alarm;
	GameObject[] startsPos;
	//游戏内的UI
	GameObject[] gameUIs;
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
	string[] perfectWord;
    bool isCoroutining = false;
    //按住蓄力协程
    IEnumerator coroutine;
    //环列表
    List<string> rings;
	//金币箱子
	[SerializeField]
	GameObject goldBox;

    //当前的目标环
   // [HideInInspector]
   // public Transform currentRing;

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

	Radar radarScript;

    //分数字典
    Dictionary<string, int> scoreDic;

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
		//PlayerPrefs.DeleteAll ();

        rig = GetComponent<Rigidbody>();
        playerColl = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
		radarScript = radar.GetComponent<Radar>();		      

        rings = new List<string>();
        scoreDic = new Dictionary<string, int>();	
		cityOffset = new Vector3 ();
		perfectWord = new string[]{ "GREAT", "GOOD", "PERFECT", "PRETTY" };

		gameUIs = GameObject.FindGameObjectsWithTag ("GameUI");
		HideGameUI (true);

		coroutine = RotateMid();
		totalCityOffset = Vector3.zero;

		InitScoreDic();

		floorNumber = 0;
		isRotate = false;
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
		SaveManager.Instance.SetVector3 ("TotalCityOffset", Vector3.zero);
		Camera.main.transform.position += initPos.position - initPrePos.position;
		Physics.gravity = new Vector3(0, gravity, 0);
		GameState = 2;
		animator.SetBool ("Idle", false);
		animator.SetBool ("Storage", true);
		animator.SetBool ("Storage", false);
		animator.SetBool ("Jump", true);
		rig.constraints = RigidbodyConstraints.None;
		transform.position = initPos.position;
	}
		

	//重新开始关卡
	//当前出生点
	public Vector3 ReGamePos = Vector3.zero;
	public void ReGame(){
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
		animator.SetBool("Jump", false);
		animator.SetBool("Storage", false);
		animator.SetBool("Dead", false);

		//删除拳套
		if (boxGloveTrans)
			Destroy (boxGloveTrans.gameObject);
	}
		

	//检查tap后的主角角度,用于开始按钮
	IEnumerator CheckRotation(){		
		while (true) {
			transform.eulerAngles += new Vector3 (-Time.deltaTime*7, 0, 0);
			if (transform.eulerAngles.x < 350 && transform.eulerAngles.x > 300) {		
				rig.constraints = RigidbodyConstraints.None;
				animator.SetBool ("Storage", false);
				animator.SetBool ("Jump", true);
				rig.AddForce (transform.up * force, ForceMode.Force);
				transform.DOLocalRotate (new Vector3 (-transform.eulerAngles.x, 0, 0), 1.5f, RotateMode.LocalAxisAdd);
				Invoke ("ReColl", 1);
				yield break;
			}
			yield return null;
		}
	}

	//随机获得一个出生点
	//该位置没有进行过”偏移“和”深度改变“
	Vector3 InitPlayerPos(){
		startsPos = GameObject.FindGameObjectsWithTag ("start");
		Vector3 startPos = startsPos [Random.Range (0, startsPos.Length)].transform.position;
		return startPos;
	}


    void Update()
    {
		if (Starting) {
			//待机状态
			//按住P开始蓄力，进入蓄力状态，碰撞取消，播放蓄力动画
			if (GameState == 0) {    				
				//if (Input.GetMouseButtonDown (0)) {
				if (Input.GetKeyDown (KeyCode.P)) {
					GameState = 1;
					playerColl.enabled = false;
					bodyColl.enabled = false;
					animator.SetBool ("Storage", true);
					animator.SetBool ("Idle", false);
				}

			}
			
        //蓄力状态
        //松开跳跃，蓄力过久死亡
        	else if (GameState == 1) {
               
				//开始改变角度
				if (isRotate == false) {
					StartCoroutine (RotateMid ());
				}

				//松开P停止蓄力，接触角色限制，改变重力，往头朝向发射，旋转一圈，进入跳跃状态，1秒后恢复碰撞，开始跳跃动画，停止旋转，摄像机扩大视野范围，然后缩小视野范围，取消警告
				if (Input.GetKeyUp (KeyCode.P)) {
					// Debug.Log(transform.eulerAngles.x - 360);
					rig.constraints = RigidbodyConstraints.None;
					Physics.gravity = new Vector3 (0, gravity, 0);
					rig.AddForce (transform.up * force, ForceMode.Force);
					transform.DOLocalRotate (new Vector3 (-transform.eulerAngles.x, 0, 0), 1.5f, RotateMode.LocalAxisAdd);

					GameState = 2;
					Invoke ("ReColl", 1);
					animator.SetBool ("Storage", false);
					animator.SetBool ("Jump", true);

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
    }

	//恢复玩家碰撞
    void ReColl(){
        playerColl.enabled = true;
		bodyColl.enabled = true;
    }

	//延迟进入主界面
	void GameOver(float delay){
		Invoke ("GameOver", delay);
	}

	//进入主界面
	public void GameOver(){
        ResetGame.Instance.OnResetBtn();
    }

    //碰撞处理
    //跳跃状态才能碰撞
    //撞到一般物体加少量分数,若超过目标点则生成新的目标点，进入待机状态，生成分数
    //身体先发生碰撞则判定死亡
    private void OnCollisionEnter(Collision coll)
    {
		//离开黑洞后状态为进入新关卡，此时若碰到一般建筑则...
		if (GameState == 7 && coll.collider.tag == "Untagged") {
			rig.constraints = RigidbodyConstraints.FreezeAll;
			//状态变为待机，播放待机动画
			GameState = 0;
			animator.SetBool("Idle", true);
			animator.SetBool("Jump", false);
			//如果是进入新关卡则增加关卡数并刷新关卡UI
			if(Level.Instance.slider.value == 1){
				PlayerPrefs.SetInt ("Level", PlayerPrefs.GetInt ("Level", 1) + 1);
				Level.Instance.UpdateLevel ();
			}
			//生成新环
			RadarScan();
			//显示关卡UI
			HideGameUI (false);
		}

		//如果是跳跃状态
        if (GameState == 2)
        {
			//如果碰到了一般建筑
            if (coll.collider.tag == "Untagged")
            {
				//获得当前碰撞的点
				currentColl = coll.contacts [0].point;
				//检查射线下是否有环
                CheckRingByRay();
				//等待一段时间根据环生成分数，生成新的环
                Invoke("ScoreGenerate", 0.05f);
				Invoke("GenerateNewRing", 0.1f);
                //一段时间后锁定主角
				rig.constraints = RigidbodyConstraints.FreezeAll;
            }

        }

		//如果蓄力过度则被拳头打飞
		if (GameState == 10) {
			//GameOverByBoxglove (transform.position + new Vector3 (0, -5, 0));
		}

		//如果被车撞了，状态变为被车撞，解除主角限制，往被撞的方向施加力，给予主角随机旋转，三秒后重开游戏
		if (coll.collider.tag == "car"&&GameState!=9) {
			GameState = 9;
			rig.constraints = RigidbodyConstraints.None;
			Vector3 carDirection= (transform.position-coll.transform.position).normalized;
			rig.AddForce((carDirection + transform.up) * carForce, ForceMode.Force);
			transform.DOLocalRotate(new Vector3(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)), 1.5f, RotateMode.LocalAxisAdd);
			Invoke("ReGame",3);
		}

    }

	void GenerateNewRing(){
		//没死的话，清除环，生成环，播放待机动画    
		if (GameState != 3) {			
			ClearRing ();
			RadarScan ();
			animator.SetBool("Idle", true);
			animator.SetBool("Jump", false);
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
		//状态变为死亡状态,解放主角，被打方向施加力，随机旋转，三秒后重置游戏
		GameState = 3;
		rig.constraints = RigidbodyConstraints.None;
		Vector3 carDirection= (transform.position-golvePos).normalized;
		rig.AddForce((carDirection + transform.up) * carForce, ForceMode.Force);
		transform.DOLocalRotate(new Vector3(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)), 1.5f, RotateMode.WorldAxisAdd);
		Invoke("ReGame",3);
		//在主角位置生成拳套，面向主角出拳
		boxGloveTrans = Instantiate (boxGlove, transform.position, Quaternion.identity).transform;
		boxGloveTrans.up = carDirection;
		boxGloveTrans.DOMove (transform.position+carDirection*2, 0.3f, false);
		//取消警告
		alarm.gameObject.SetActive (false);
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
		}
    }

	//往黑洞中间施加力，之后改为被黑洞吸引
	IEnumerator AddForceInHole(){
		int i = 3;
		while (i <= 0) {
			i--;
			rig.AddForce ((holeTarget.position - transform.position)*50);
			yield return new WaitForSeconds (0.2f);
		}
	}		

	[HideInInspector]
	//记录进入黑洞后的玩家位置
	public Vector3 transformPre = Vector3.zero;
	void OnTriggerExit(Collider other)
	{
		//如果离开了黑洞
		if (other.tag == "hole")
		{
			GameObject[] inholes = GameObject.FindGameObjectsWithTag ("Player");
			foreach (GameObject go in inholes) {
				go.layer = 0;
			}

			//删除存在的黑洞
			if (firstHole) {
				firstHole.SetActive(false);
			}
			if(holeTarget){
				Destroy(holeTarget.Find("Hole").gameObject);
			}

			//记录下降层数
			//radarScript.floorNumber = PlayerPrefs.GetInt ("Floor", 0);
			//PlayerPrefs.SetInt ("Floor", floorNumber);
			floorNumber++;

			//锁定主角的X和Z坐标
			rig.constraints = RigidbodyConstraints.FreezePositionX;
			rig.constraints = RigidbodyConstraints.FreezePositionZ;

			//反向移动城市
			//穿过黑洞后城镇往下移动100米，获得一个随机出生点
			city.transform.position += new Vector3(0, -100, 0);
			nextLevelPos = InitPlayerPos ();
			//进入第一层时,历史记录为空则随机生成，否则使用历史记录
			//第二层开始一直取随机位置
			if (floorNumber == 1) {
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

	//通过射线检测主角下方处是否含有环，有则添加进环数组，环的碰撞取消
    void CheckRingByRay(){
		Ray ray = new Ray(transform.position+new Vector3(0,5,0), Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach(RaycastHit hit in hits){
            string hitName = hit.collider.name;
            if(hitName.StartsWith("ring")){
                rings.Add(hitName);
                hit.collider.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }		

    //生成分数,还需要生成文字perfect提示
    void ScoreGenerate(){	
		//如果没有踩中环,弹出MISS并拳头打飞
		if (CheckRing () == "normal") {				
			GameOverByBoxglove (currentColl);
			TipPop.GenerateTip ("MISS", 0.5f);					
			return;		
		}
		//踩中环的时候状态变为待机状态,生成对应的分数，删除踩中的环
		GameState = 0;
		TipPop.GenerateTip("+"+scoreDic[CheckRing()], 0.5f);
		rings.Clear();
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
    bool isRotate;
    IEnumerator RotateMid()
	{		
        if (!isRotate)
		{
            isRotate = true;
			float MaxAngle = 0;
			float totalAngle = 0;
			float speed = -1f;
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
				yield return null;
            }
        }
    }
		

    //初始化环分数
    void InitScoreDic(){
        scoreDic.Add("ring1", 300);
        scoreDic.Add("ring2", 200);
        scoreDic.Add("ring3", 100);
        scoreDic.Add("normal", 1);
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
		
}
