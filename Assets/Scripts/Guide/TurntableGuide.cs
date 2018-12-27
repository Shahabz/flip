using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TurntableGuide : MonoBehaviour {
	public GameObject home;
	public GameObject turntable;
	public GameObject menuMove;
	public GameObject shop;
	public GameObject buy;
	public Transform bg;
	public Button awesomeBtn;

	public GameObject[] point;
	public GameObject finger;

	public Shop shopScript;

	static TurntableGuide instance;
	public static TurntableGuide Instance{
		get { return instance; }
	}   
	private void Awake()
	{
		instance = this;
	}

	void Start(){
		if (PlayerPrefs.GetInt ("TurnHomeFinish", 0) == 1) {
			bg.gameObject.SetActive (true);
			GameObject turnGO;
			turnGO = Instantiate (turntable, bg);
			turnGO.SetActive (true);
			Transform turnBtn = turnGO.transform.Find ("Button");
			point [1].SetActive (true);

			turnBtn.GetComponent<Button>().onClick.AddListener (()=>{
				PlayerPrefs.SetInt("TurnHomeFinish",2);
				bg.gameObject.SetActive (false);
				Menu.Instance.turnTable.SetActive(true);
				point[1].SetActive(false);
				awesomeBtn.onClick.AddListener(()=>{
					Destroy(turnGO);
					bg.gameObject.SetActive (true);
					point[2].SetActive(true);
					GameObject moveGO = Instantiate (menuMove, bg);
					moveGO.GetComponent<Button>().onClick.AddListener(()=>{
						Destroy(moveGO);
						GameObject shopGo;
						point [2].SetActive (false);
						Invoke("ShowShop",0.5f);
					});
				});
			});				
		}
		if (PlayerPrefs.GetInt ("TurnHomeFinish", 0) == 3&&SceneManager.GetActiveScene().name == "Shop") {
			bg.gameObject.SetActive (true);
			finger.SetActive (true);
			StartCoroutine (CheckFingerMove ());
			PlayerPrefs.SetInt ("TurnHomeFinish", 4);
		}
	}

	IEnumerator CheckFingerMove(){
		while (true) {
			if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved)||Input.GetKeyDown(KeyCode.Z)) {					
				if (Application.platform == RuntimePlatform.IPhonePlayer||Application.platform == RuntimePlatform.Android) {
					Vector2 touchDelPos = Input.GetTouch(0).deltaPosition;
					if (touchDelPos.x < -7) {
						shopScript.RotateCy (true);
						finger.SetActive (false);
						if (PlayerPrefs.GetInt ("vibration", 1) == 1)
							MultiHaptic.HapticLight ();
						yield return new WaitForSeconds (0.3f);
						point [0].SetActive (true);
						bg.gameObject.SetActive(false);
						GameObject buyGO = Instantiate (buy, bg);
						buyGO.transform.Find("bg").GetComponent<Button> ().onClick.AddListener (() => {
							Destroy (buyGO);
							point [0].SetActive (false);
						});
						yield break;
					}
				}
				else{
					shopScript.RotateCy (true);
					finger.SetActive (false);
					if (PlayerPrefs.GetInt ("vibration", 1) == 1)
						MultiHaptic.HapticLight ();
					yield return new WaitForSeconds (0.3f);
					point [0].SetActive (true);
					bg.gameObject.SetActive(false);
					GameObject buyGO = Instantiate (buy, bg);
					buyGO.transform.Find("bg").GetComponent<Button> ().onClick.AddListener (() => {
						Destroy (buyGO);
						point [0].SetActive (false);
					});
					yield break;
				}
			}
			yield return null;
		}
	}

	void ShowShop(){
		Instantiate(shop,shop.transform.position,shop.transform.rotation,bg);
		point [3].SetActive (true);
	}

	public void StartGuide () {
		bg.gameObject.SetActive (true);
		GameObject homeGO = Instantiate (home,bg);
		homeGO.SetActive (true);
		if (homeGO.activeSelf) {			
			PlayerPrefs.SetInt ("TurnHomeFinish", 1);
			point [0].SetActive (true);
		}
	}
	
	void Update(){

	}
}
