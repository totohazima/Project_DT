using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickUI : MonoBehaviour
{
    public enum ContentsUI
    {
        INVENTORY,
    }

    public ContentsUI contents;
    public RectTransform popupTransform;
    public GameObject popupPrefab;

    public void PopUpUI()
    {
        switch(contents)
        {
            case ContentsUI.INVENTORY:
                PoolManager.instance.Spawn(popupPrefab, Vector3.zero, Vector3.one, Quaternion.identity, true, popupTransform);
                break;
        }
         
    }
}
