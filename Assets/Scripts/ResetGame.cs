using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour {

    static ResetGame instance;
    public static ResetGame Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        instance = this;
    }

    public void OnResetBtn(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
