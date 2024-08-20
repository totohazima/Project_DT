using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GDBA;
using GameInventory;
using StatusHelper;
using System.Linq;
public class BasicUI : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private GameMoneyUI goldUI;
    private void Start()
    {
        if (GameManager.instance != null)
        {
            gameManager = GameManager.instance;
        }
    }

    private void Update()
    {
        UIVisualize();
    }
    public void UIVisualize()
    {
        UI_GoldSetting();
    }
    private void UI_GoldSetting()
    {
        goldUI.ui_Text.text = GameManager.instance.gameDataBase.playerInfo.Gold.ToString("F0");
    }
}
