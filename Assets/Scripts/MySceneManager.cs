using UnityEngine;
using UnityEngine.SceneManagement;


public class MySceneManager : MonoBehaviour {

	public static void LoadSceneAdd(string name){
		SceneManager.LoadScene (name, LoadSceneMode.Additive);
	}

	public static void LoadScene(string name){
		SceneManager.LoadScene (name, LoadSceneMode.Single);
	}

	public static void LoadSceneSyn(string name){
		SceneManager.LoadSceneAsync (name, LoadSceneMode.Single);
	}
}
