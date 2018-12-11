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

	string[] perfectWord;
    //
    bool isCoroutining = false;

    //按住蓄力协程
    IEnumerator coroutine;

    //环列表
    List<string> rings;

    //当前的目标环
   // [HideInInspector]
   // public Transform currentRing;

	//黑洞存在
	[HideInInspector]
	public bool isHoling = false;
	[HideInInspector]
	public Transform holeTarget;
	//获取环的位置
	public Vector3 ringPos;

	//关卡起始位置
	[HideInInspector]
	public Vector3 nextLevelPos;

	Vector3 currentColl;

	//城市移动的偏差
	Vector3 cityOffset;

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

	//int floorNumber = 0;

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

		//transform.position = InitPlayerPos();

        rig = GetComponent<Rigidbody>();
        playerColl = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();

        coroutine = RotateMid();

        rings = new List<string>();
        scoreDic = new Dictionary<string, int>();	
        InitScoreDic();
		cityOffset = new Vector3 ();

        //RadarScan();
		radarScript = radar.GetComponent<Radar>();

		perfectWord = new string[]{ "GREAT", "GOOD", "PERFECT", "PRETTY" };

		gameUIs = GameObject.FindGameObjectsWithTag ("GameUI");
		HideGameUI (true);
		//floorNumber = 0;

	}

	//tap开始游戏
	public void GameStart(){		
		Physics.gravity = new Vector3(0, gravity, 0);
		playerColl.enabled = false;
		bodyColl.enabled = false;
		GameState = 2;
		animator.SetBool ("Storage", true);
		animator.SetBool ("Idle", false);
		StartCoroutine (CheckRotation ());
	}
		

	//重新开始关卡
	public void ReGame(){
		//重置位置
		Debug.Log (SaveManager.Instance.ReadLevelPos () [0]);
		transform.position = SaveManager.Instance.ReadLevelPos () [0];

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

	//检查tap后的主角角度
	IEnumerator CheckRotation(){		
		while (true) {
			transform.eulerAngles += new Vector3 (-Time.deltaTime*7, 0, 0);
			if (transform.eulerAngles.x < 353 && transform.eulerAngles.x > 300) {		
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

	Vector3 InitPlayerPos(){
		startsPos = GameObject.FindGameObjectsWithTag ("start");
		Vector3 startPos = startsPos [Random.Range (0, startsPos.Length)].transform.position;
		return startPos;
	}

    void Update()
    {
		if (Starting) {
			//待机状态
			//按住蓄力
			if (GameState == 0) {    
				// if (Input.GetKeyDown(KeyCode.Space))
				//if (Input.GetMouseButtonDown (0)) {
				if (Input.GetKeyDown(KeyCode.P)) {
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
				//if (Input.GetKeyUp(KeyCode.Space))
				//蓄力过久警告
				if (transform.eulerAngles.x < 340 && transform.eulerAngles.x > 250) {
				
					StartCoroutine (AlarmStorage ());
				}

				//if (transform.eulerAngles.x < 300 && transform.eulerAngles.x > 250) {
//					rig.constraints = RigidbodyConstraints.None;
//					GameState = 10;
//					playerColl.enabled = true;
//					bodyColl.enabled = true;
//					animator.SetBool ("Storage", false);
//					animator.SetBool ("Dead", true);
				//} else {
					// transform.Rotate(new Vector3(1, 0, 0), -0.5f);
               
					StartCoroutine (coroutine);

				//}



				//if (Input.GetMouseButtonUp (0)) {
				if (Input.GetKeyUp(KeyCode.P)) {
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
					StopCoroutine (coroutine);

					Camera.main.DOFieldOfView (70, 1).OnComplete (() => {
						Camera.main.DOFieldOfView (60, 1);
					});

					alarm.gameObject.SetActive (false);
					isAlarming = false;
				}

			}
        //跳跃状态
        //可以发生碰撞
        else if (GameState == 2) {
           

			}
        //死亡状态
        //延迟2秒重开游戏
        else if (GameState == 3) {
				//Invoke("GameOver", 2);
       		
			}
        //获胜状态
        //
        else if (GameState == 4) {
           
			}
        //结算状态
        //
        else if (GameState == 5) {
        
			}
        //进入黑洞
        //
        else if (GameState == 6) {

				// Destroy(GameObject.FindWithTag("DeadPlane"));

				// Invoke("EnterNewGame", 3);

			}
        //抵达新关卡
        //
        else if (GameState == 7) {
           
			}
        //转向环心
        //
        else if (GameState == 8) {

			}
		}
    }

    void ReColl(){
        playerColl.enabled = true;
		bodyColl.enabled = true;
    }

	void GameOver(float delay){
		Invoke ("GameOver", delay);
	}

    void GameOver(){
        ResetGame.Instance.OnResetBtn();
    }

    //碰撞处理
    //跳跃状态才能碰撞
    //撞到一般物体加少量分数,若超过目标点则生成新的目标点，进入待机状态，生成分数
    //身体先发生碰撞则判定死亡

    private void OnCollisionEnter(Collision coll)
    {
		if (GameState == 7 && coll.collider.tag == "Untagged") {
			transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z), 0.2f, RotateMode.Fast).OnComplete(() =>
				{
					transform.DOKill(false);
					if(gameObject.layer == 0){
						rig.constraints = RigidbodyConstraints.FreezeAll;
					}
				});
			GameState = 0;

			animator.SetBool("Idle", true);
			animator.SetBool("Jump", false);

			//nextLevelPos = new Vector3 (nextLevelPos.x, transform.position.y, nextLevelPos.z);

			if(Level.Instance.slider.value == 1){
				PlayerPrefs.SetInt ("Level", PlayerPrefs.GetInt ("Level", 1) + 1);
				Level.Instance.UpdateLevel ();

			}

			RadarScan();

			HideGameUI (false);
		}
        if (GameState == 2)
        {
            if (coll.collider.tag == "Untagged")
            {
				currentColl = coll.contacts [0].point;

                CheckRingByRay();
                
                GameState = 0;
                Invoke("ScoreGenerate", 0.05f);
				Invoke("GenerateNewRing", 0.1f);
                
				transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z), 0.2f, RotateMode.Fast).OnComplete(() =>
					{
						transform.DOKill(false);
						if(gameObject.layer == 0){
							if(GameState!=3)
								rig.constraints = RigidbodyConstraints.FreezeAll;

						}
					});                
            }

        }

		if (GameState == 10) {
			//ReGame ();
			GameOverByBoxglove (transform.position + new Vector3 (0, -5, 0));
		}

		if (coll.collider.tag == "car"&&GameState!=9) {
			GameState = 9;
			rig.constraints = RigidbodyConstraints.None;
			Vector3 carDirection= (transform.position-coll.transform.position).normalized;
			rig.AddForce((carDirection + transform.up) * carForce, ForceMode.Force);

			transform.DOLocalRotate(new Vector3(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)), 1.5f, RotateMode.LocalAxisAdd);

			//GameOver (3);
			Invoke("ReGame",3);

		}

    }

	void GenerateNewRing(){
		//生成新的环    
		if (GameState != 3) {			
			ClearRing ();
			RadarScan ();
			animator.SetBool("Idle", true);
			animator.SetBool("Jump", false);
		}
	}

	Transform boxGloveTrans;
	void GameOverByBoxglove(Vector3 golvePos){
		if (GameState == 3) {
			return;
		}
		GameState = 3;
		rig.constraints = RigidbodyConstraints.None;
		Vector3 carDirection= (transform.position-golvePos).normalized;
		rig.AddForce((carDirection + transform.up) * carForce, ForceMode.Force);
		transform.DOLocalRotate(new Vector3(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)), 1.5f, RotateMode.WorldAxisAdd);

		//GameOver (3);
		Invoke("ReGame",3);

		boxGloveTrans = Instantiate (boxGlove, transform.position, Quaternion.identity).transform;
		boxGloveTrans.up = carDirection;
		boxGloveTrans.DOMove (transform.position+carDirection*2, 0.3f, false);

		alarm.gameObject.SetActive (false);
	}


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
		if (other.tag == "hole") {
			StartCoroutine (AddForceInHole ());
			//GameState = 6;
			//StartCoroutine (_debug());
		}
    }

	IEnumerator _debug(){
		while (true) {
			Debug.Log (GameState);
			yield return new WaitForSeconds(0.2f);
		}
	}

	IEnumerator AddForceInHole(){
		int i = 3;
		while (i <= 0) {
			i--;
			rig.AddForce ((holeTarget.position - transform.position)*50);
			yield return new WaitForSeconds (0.2f);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "hole")
		{

			//记录下降层数
			//radarScript.floorNumber = PlayerPrefs.GetInt ("Floor", 0);
			//PlayerPrefs.SetInt ("Floor", floorNumber);
			//floorNumber++;

			//通关
			if (Level.Instance.slider.value == 1) {
				//进入新关卡清除之前关卡的信息
				SaveManager.Instance.ClearPosList ();
				radarScript.levelPos.Clear ();

				//存储起始位置
				nextLevelPos = InitPlayerPos ();
				PlayerPrefs.SetFloat ("nextLevelPosX", nextLevelPos.x);
				PlayerPrefs.SetFloat ("nextLevelPosZ", nextLevelPos.z);
				radarScript.levelPos.Add (nextLevelPos);
				//计算城市偏差
				//cityOffset = transform.position-new Vector3(nextLevelPos.x,0, nextLevelPos.x);
				//起始位置对应在第一层的位置
				radarScript.levelPos.Add (nextLevelPos + new Vector3 (cityOffset.x, 50, cityOffset.z));
				SaveManager.Instance.SaveLevelPos (radarScript.levelPos);

				//清除黑洞
				if(holeTarget){
					Destroy (holeTarget.gameObject);
				}

			}


			city.transform.position += new Vector3(0, -100, 0);

			if (SaveManager.Instance.ReadLevelPos ().Count > 0) {
				nextLevelPos = SaveManager.Instance.ReadLevelPos () [1];
			} else {
				nextLevelPos = InitPlayerPos ();
				PlayerPrefs.SetFloat ("nextLevelPosX", nextLevelPos.x);
				PlayerPrefs.SetFloat ("nextLevelPosZ", nextLevelPos.z);
			}

			//transform.localPosition = new Vector3 (nextLevelPos.x, transform.localPosition.y, nextLevelPos.z);

			//移动人物到目标点上方
//			transform.DOLocalMoveX (nextLevelPos.x, 0.1f, false).SetDelay(0.3f);
//			transform.DOLocalMoveZ (nextLevelPos.z, 0.1f, false).SetDelay(0.3f).OnComplete(()=>{
//				//transform.DORotate (new Vector3 (transform.eulerAngles.x, 180, transform.eulerAngles.z), 2f, RotateMode.Fast);
//				rig.constraints = RigidbodyConstraints.FreezePositionX;
//				rig.constraints = RigidbodyConstraints.FreezePositionZ;
//			});

			//反向移动城市
			rig.constraints = RigidbodyConstraints.FreezePositionX;
			rig.constraints = RigidbodyConstraints.FreezePositionZ;
			//Debug.Log (transform.position);
			//Debug.Log (nextLevelPos);
			//Debug.Log (transform.position - nextLevelPos);
			//cityOffset = transform.position-nextLevelPos;
			cityOffset = transform.position-new Vector3(PlayerPrefs.GetFloat ("nextLevelPosX", nextLevelPos.x),0,PlayerPrefs.GetFloat ("nextLevelPosZ", nextLevelPos.x));
			Vector3 cityPos = city.transform.position;
			city.transform.DOLocalMoveX (cityPos.x+cityOffset.x, 0.1f, false).SetDelay(0.3f);
			city.transform.DOLocalMoveZ (cityPos.z+cityOffset.z, 0.1f, false).SetDelay(0.3f).OnComplete(()=>{
				//纠正人物方向
				//transform.DORotate (new Vector3 (transform.eulerAngles.x, 180, transform.eulerAngles.z), 2f, RotateMode.Fast);
			});

			//玄幻黑洞
//			BlackHole blackHole = Camera.main.GetComponent<BlackHole> ();
//			if (blackHole) {
//				blackHole.enabled = false;
//			}

		}
	}

	void ExitHole(){
		GameState = 2;
	}

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

    //生成分数
    void ScoreGenerate(){		
		if (CheckRing () == "normal") {	
			
			if (Vector3.Distance (ringPos, transform.position) > 2) {
				GameOverByBoxglove (currentColl);
				TipPop.GenerateTip ("MISS", 0.5f);					
				return;
			}
		

//			GameOverByBoxglove (currentColl);
//			TipPop.GenerateTip("MISS", 0.5f);
//			return;
		}

		TipPop.GenerateTip("+"+scoreDic[CheckRing()], 0.5f);
//		TipPop.GenerateTip(perfectWord[Random.Range(0,perfectWord.Length)], 0.5f);

		rings.Clear();
    }

	void GenerateScoreDaily(){
		TipPop.GenerateTip("+"+scoreDic[CheckRing()], 0.5f);
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
    bool isRotate = false;
    IEnumerator RotateMid()
	{
		
        if (!isRotate)
		{
            isRotate = true;
            while (true)
			{
				float speed = -0.1f;

				if (transform.eulerAngles.x < 300 && transform.eulerAngles.x > 250) {
					//transform.Rotate(new Vector3(1, 0, 0), 0.1f);
					speed = 0.1f;
				} else if(transform.eulerAngles.x>=355){
					//transform.Rotate(new Vector3(1, 0, 0), -0.1f);
					speed = -0.1f;
				}

				transform.Rotate(new Vector3(1, 0, 0), speed);
                //yield return null;
                yield return new WaitForSeconds(0.1f);
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

    void RadarScan()
    {
        radar.SetActive(true);
        Invoke("ScanOver", 0.2f);
    }

    void ScanOver(){
        radar.SetActive(false);
    }

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

	void HideGameUI(bool hide){
		foreach (GameObject go in gameUIs) {
			go.SetActive (!hide);
		}
	}
		
}
