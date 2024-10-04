using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI")]
    public CharacterList_UI characterList_UI = null;
    private void Awake()
    {
        Instance = this;    
    }
}
