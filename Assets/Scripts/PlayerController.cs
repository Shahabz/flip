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
    [HideInInspector]
    public int GameState = 0;

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
        rig = GetComponent<Rigidbody>();
        playerColl = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();

        coroutine = RotateMid();

        rings = new List<string>();
        scoreDic = new Dictionary<string, int>();
        InitScoreDic();

        RadarScan();

	}

    void Update()
    {
        //待机状态
        //按住蓄力
        if (GameState == 0)
        {    
           // if (Input.GetKeyDown(KeyCode.Space))
            if(Input.GetMouseButtonDown(0))
            {
                GameState = 1;
                playerColl.enabled = false;
				bodyColl.enabled = false;
                animator.SetBool("Storage", true);
                animator.SetBool("Idle", false);
            }

        }
        //蓄力状态
        //松开跳跃，蓄力过久死亡
        else if (GameState == 1)
        {
            //if (Input.GetKeyUp(KeyCode.Space))

            if(transform.eulerAngles.x<300&& transform.eulerAngles.x>250)
            {
                rig.constraints = RigidbodyConstraints.None;
                GameState = 3;
                playerColl.enabled = true;
				bodyColl.enabled = true;
                animator.SetBool("Storage", false);
                animator.SetBool("Dead", true);
            }
            else{
                // transform.Rotate(new Vector3(1, 0, 0), -0.5f);
               
                StartCoroutine(coroutine);

            }

            if (Input.GetMouseButtonUp(0))
            {
                // Debug.Log(transform.eulerAngles.x - 360);
                rig.constraints = RigidbodyConstraints.None;
                Physics.gravity = new Vector3(0, gravity, 0);
                rig.AddForce(transform.up * force, ForceMode.Force);
                transform.DOLocalRotate(new Vector3(-transform.eulerAngles.x, 0, 0), 1.5f, RotateMode.LocalAxisAdd);

                GameState = 2;
				Invoke("ReColl", 1);
                animator.SetBool("Storage", false);
                animator.SetBool("Jump", true);

                isRotate = false;
                StopCoroutine(coroutine);


            }

        }
        //跳跃状态
        //可以发生碰撞
        else if (GameState == 2)
        {
           

        }
        //死亡状态
        //延迟2秒重开游戏
        else if (GameState == 3)
        {
            //Invoke("GameOver", 2);
       		
        }
        //获胜状态
        //
        else if (GameState == 4)
        {
           
        }
        //结算状态
        //
        else if (GameState == 5)
        {
        
        }
        //进入黑洞
        //
        else if(GameState == 6){

           // Destroy(GameObject.FindWithTag("DeadPlane"));

           // Invoke("EnterNewGame", 3);

        }
        //抵达新关卡
        //
        else if(GameState == 7){
           
        }
        //转向环心
        //
        else if(GameState == 8){

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
			transform.DORotate(new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z), 0.2f, RotateMode.Fast).OnComplete(() =>
				{
					transform.DOKill(false);
					if(gameObject.layer == 0){
						rig.constraints = RigidbodyConstraints.FreezeAll;
					}
				});
			GameState = 0;

			animator.SetBool("Idle", true);
			animator.SetBool("Jump", false);
			RadarScan();
		}
        if (GameState == 2)
        {
            if (coll.collider.tag == "Untagged")
            {


                CheckRingByRay();
                //ScoreGenerate();
                //coll.collider.tag = "Untagged";
                //HouseGenerate.Instance.GenerateHouse(coll.transform.position);
                transform.DORotate(new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z), 0.2f, RotateMode.Fast).OnComplete(() =>
                {
                   		transform.DOKill(false);
						if(gameObject.layer == 0){
							rig.constraints = RigidbodyConstraints.FreezeAll;
						}
                });
                GameState = 0;
                Invoke("ScoreGenerate", 0.1f);
                animator.SetBool("Idle", true);
                animator.SetBool("Jump", false);

                //生成新的环                
                ClearRing();
                RadarScan();

            }

            //if (coll.collider.name == "DeadPlane")
            //{
                //GameState = 3;
                //Destroy(coll.gameObject);
                //animator.SetBool("Dead", true);
                //animator.SetBool("Jump", false);
            //}
        }

		if (coll.collider.tag == "car"&&GameState!=9) {
			GameState = 9;
			rig.constraints = RigidbodyConstraints.None;
			Vector3 carDirection= (transform.position-coll.transform.position).normalized;
			rig.AddForce((carDirection + transform.up) * carForce, ForceMode.Force);

			transform.DOLocalRotate(new Vector3(Random.Range(0,360), Random.Range(0,360), Random.Range(0,360)), 1.5f, RotateMode.WorldAxisAdd);
			GameOver (3);

			//RagdollDead.Instance.ChangeToDead ();
		}

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
			//StartCoroutine (debug());
		}
    }

	IEnumerator debug(){
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
			//Vector3 offSet = new Vector3(10, 0, 10);
			city.transform.position += new Vector3(0, -50, 0);
			//Invoke ("ExitHole", 2);

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
        TipPop.GenerateTip("+"+scoreDic[CheckRing()], 0.5f);
    }

    //判断踩中的环
    string CheckRing(){
		if(rings.Contains("ring1(Clone)")||rings.Contains("ring1_mid(Clone)")||rings.Contains("ring1_small(Clone)")){
            rings.Clear();
            return "ring1";
		}else if(rings.Contains("ring2(Clone)")||rings.Contains("ring2_mid(Clone)")||rings.Contains("ring2_small(Clone)")){
            rings.Clear();
            return "ring2";
		}else if (rings.Contains("ring3(Clone)")||rings.Contains("ring3_mid(Clone)")||rings.Contains("ring3_small(Clone)")){
            rings.Clear();
            return "ring3";
        }else{
            rings.Clear();
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
                transform.Rotate(new Vector3(1, 0, 0), -0.1f);
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
}
