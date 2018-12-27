using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour {
	//存每个动作的父对象
	public GameObject[] skills;
	//存每个动作的等级
	Button[] lvUps;
	//存每个动作的选择按钮
	Button[] selects;
	//存每个动作的选择状态对象
	GameObject[] selectsGO;
	//存每个动作的技能
	List<GameObject[]> skillActives;
	//存每个动作的升级金币
	int[] lvUpGolds;
	Text[] lvUpGoldTexts;
	//存每个动作的初始升级金币
	int[] lvUpStartGolds;

	[SerializeField]
	Animator animator;

	int length;
	void Start () {		
		Init ();
		UpdateUI ();
	}

	//获取所有的动作相关对象
	void Init(){
		//skills = GameObject.FindGameObjectsWithTag ("skill");
		length = skills.Length;
		lvUpGolds = new int[length];
		lvUps = new Button[length];
		selects = new Button[length];
		lvUpGoldTexts = new Text[length];
		selectsGO = new GameObject[length];
		lvUpStartGolds = new int[]{281,1244,9882,77655,1244888};
		skillActives = new List<GameObject[]> ();

		for (int i = 0; i < length; i++) {
			Transform skillTrans = skills [i].transform;
			lvUps[i] = skillTrans.Find ("Upgrade").Find ("Upgrade").GetComponent<Button>();
			selects [i] = skillTrans.Find ("action").GetComponent<Button>();
			selectsGO[i] = skillTrans.Find ("action").Find ("select").gameObject;
			lvUpGoldTexts[i] = skillTrans.Find ("Upgrade").Find ("gold").GetComponent<Text>();

			GameObject[] skillActivesGO = new GameObject[5];
			for (int j = 1; j <= 5; j++) {				
				skillActivesGO [j-1] = skillTrans.Find ("LvFalse" + j).Find("LvTrue"+j).gameObject;
			}
			skillActives.Add (skillActivesGO);
		}
			
	}
		

	//更新所有的UI
	public void UpdateUI(){
		UpdateSelect ();
		for (int i = 0; i < length; i++) {
			UpdateSkillLevel (i);
		}
		UpdateSkillGold ();
		UpdateSkillBtn ();
	}

	//更新状态存储在curSelect
	//更新选择状态
	void UpdateSelect(){
		int curSelect = PlayerPrefs.GetInt ("curSelect", 0);
		for (int i = 0; i < selectsGO.Length; i++) {
			if (curSelect == i) {
				selectsGO [i].SetActive (true);
			} else {
				selectsGO [i].SetActive (false);
			}
		}
	}

	//技能等级存储在curLevel0，curLevel1，curLevel2，curLevel3，curLevel4
	//更新特定技能的等级状态
	void UpdateSkillLevel(int index){
		int curLevel = 0;
		if (index == 0) {
			curLevel = PlayerPrefs.GetInt ("curLevel" + index, 1);
		} else {
			curLevel = PlayerPrefs.GetInt ("curLevel" + index, 0);
		}
		for(int i=0;i<=curLevel-1;i++){
			skillActives [index] [i].SetActive (true);
		}
		for (int i = curLevel; i < 5; i++) {
			skillActives [index] [i].SetActive (false);
		}
	}

	//金币存储在SkillGold1，SkillGold2，SkillGold3，SkillGold4，SkillGold5
	//更新技能升级金币
	void UpdateSkillGold(){
		for (int i = 0; i < length; i++) {
			int gold = PlayerPrefs.GetInt ("SkillGold" + i, lvUpStartGolds [i]);
			lvUpGolds [i] = gold;
			lvUpGoldTexts [i].text = "$" + gold;

			int curLevel = 0;
			curLevel = PlayerPrefs.GetInt ("curLevel" + i, 0);
			if (curLevel >= 5) {
				lvUpGoldTexts [i].text = "MAX";
			}

		}
	}

	//更新升级按钮
	void UpdateSkillBtn(){
		int gold = PlayerPrefs.GetInt ("Gold", 0);
		for (int i = 0; i < length; i++) {
			int curLevel = 0;
			if (i == 0) {
				curLevel = PlayerPrefs.GetInt ("curLevel" + i, 1);
			} else {
				curLevel = PlayerPrefs.GetInt ("curLevel" + i, 0);
			}
			if (gold >= lvUpGolds [i]&&curLevel<5) {
				lvUps [i].interactable = true;
			} else {
				lvUps [i].interactable = false;
			}
		}
	}



	//点击技能升级,扣钱保存，刷新金币存，刷新选择状态存,刷新等级状态存，刷新升级按钮
	public void OnSkillBtn(int index){		
		int gold = PlayerPrefs.GetInt ("Gold", 0);
		Gold.Instance.UseGold (lvUpGolds [index]);
		MoneyManager.Instance.UpdateGold ();
		lvUpGolds [index] *= 2;
		lvUpGoldTexts [index].text = "$" + lvUpGolds [index];
		PlayerPrefs.SetInt ("SkillGold" + index, lvUpGolds [index]);
		if (index == 0) {
			PlayerPrefs.SetInt ("curLevel" + index, PlayerPrefs.GetInt ("curLevel" + index, 1) + 1);
		} else {
			PlayerPrefs.SetInt ("curLevel" + index, PlayerPrefs.GetInt ("curLevel" + index, 0) + 1);
		}
		UpdateSkillLevel (index);
		UpdateSkillBtn ();
		UpdateSkillGold ();
		OnSelectBtn (index);
	}

	//点击人物选择
	public void OnSelectBtn(int index){
		if (PlayerPrefs.GetInt ("curLevel" + index, 0) > 0) {
			PlayerPrefs.SetInt ("curSelect", index);
			UpdateSelect ();
		} else {
			TipPop.GenerateTip ("Not unlocked", 0.5f);
		}

	}


}
