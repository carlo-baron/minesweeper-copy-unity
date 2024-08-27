using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    #region Start Variables
    public static bool isStart = true;
    #endregion

    void Awake()
    {
        if(instance != null){
            Destroy(gameObject);
        }else{
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
