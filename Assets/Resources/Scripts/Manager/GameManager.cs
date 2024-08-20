using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDBA;
using UnityEditor;
using GameSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameDataBase gameDataBase;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        ScriptInit();
        
        //LoadingManager.instance.LoadScene("Stage");
    }

    private void ScriptInit()
    {
        gameDataBase.InitInstance();
        PoolManager.InitInstance();    
    }
}
    
