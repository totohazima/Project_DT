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

    /// <summary>
    /// 랜덤 기능 함수
    /// </summary>
    public int Judgment(float[] rando)
    {
        int count = rando.Length;
        float max = 0;
        for (int i = 0; i < count; i++)
            max += rando[i];

        float range = UnityEngine.Random.Range(0f, (float)max);
        //0.1, 0.2, 30, 40
        double chance = 0;
        for (int i = 0; i < count; i++)
        {
            chance += rando[i];
            if (range > chance)
                continue;

            return i;
        }

        return -1;
    }
}
    
