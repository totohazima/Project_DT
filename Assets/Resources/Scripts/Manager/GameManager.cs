using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDBA;
using UnityEditor;
using GameSystem;
using UnityEngine.U2D;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameDataBase gameDataBase;

    [Header("Manager")]
    public EffectManager effectManager;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        ScriptInit();

        Application.targetFrameRate = 60;
        //LoadingManager.instance.LoadScene("Stage");
    }

    private void ScriptInit()
    {
        gameDataBase.InitInstance();
        PoolManager.InitInstance();    
    }

    /// <summary>
    /// ���� ��� �Լ�
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


    /// <summary>
    /// ���������� �������� ���� ������
    /// </summary>
    /// <param name="minNumber">������� �����ð��� Ex)0���� NONE�� ��� �������� �ʰ��ϱ� ����</param>
    public T GetRandomEnumValue<T>(int minNumber, int maxNumber)
    {
        T[] values = (T[])System.Enum.GetValues(typeof(T));

        // ���� �ε����� �ϳ��� ����
        return values[Random.Range(minNumber, maxNumber + 1)];
    }
}
    
