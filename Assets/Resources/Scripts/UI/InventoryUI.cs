using GameSystem;
using GDBA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour, ICustomUpdateMono
{
    public enum InventoryTab
    {
        GAMEMONEY_TAB = 0,
        EQUIPMENT_TAB = 1,
    }
    public enum GameMoneyTab
    {
        GOLD_UI = 0,
        RUBY_UI = 1,
    }

    public InventoryTab currentShowTab;
    public GameObject selectedEffect;
    public RectTransform slotMenu;
    public RectTransform gameMoneyUI;
    public EquipmentUsableUI equipmentUsableUI;
    public RectTransform[] slotGroups;
    protected List<GameMoneySlot> gameMoneySlots = new List<GameMoneySlot>();
    protected List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();
    [Header("정렬 기준")]
    [Tooltip("false: 내림차순, true: 오름차순")]public bool isSortByAscending;

    [HideInInspector] public List<GameObject> effects;
    private List<GameObject> inventoryScrolls = new List<GameObject>();
    private List<GameMoneyUI> moneyUIs = new List<GameMoneyUI>();
    private new Transform transform;
    private RectTransform rectTransform;

    private void Awake()
    {
        transform = gameObject.transform;
        rectTransform = GetComponent<RectTransform>();
        for(int i = 0; i < slotMenu.childCount; i++)
        {
            inventoryScrolls.Add(slotMenu.GetChild(i).gameObject);
        }
        for(int i = 0; i < gameMoneyUI.childCount; i++)
        {
            moneyUIs.Add(gameMoneyUI.GetChild(i).GetComponent<GameMoneyUI>());
        }

        for(int i = 0; i < slotGroups.Length; i++)
        {
            switch(i)
            {
                case (int)InventoryTab.GAMEMONEY_TAB:
                    for(int j = 0; j < slotGroups[i].childCount; j++)
                    {
                        gameMoneySlots.Add(slotGroups[i].GetChild(j).GetComponent<GameMoneySlot>());
                    }
                    break;
                case (int)InventoryTab.EQUIPMENT_TAB:
                    for (int j = 0; j < slotGroups[i].childCount; j++)
                    {
                        equipmentSlots.Add(slotGroups[i].GetChild(j).GetComponent<EquipmentSlot>());
                    }
                    break;
            }
        }
    }
    private void OnEnable()
    {
        rectTransform.anchoredPosition = Vector3.zero;
        rectTransform.sizeDelta = Vector3.zero;
        CustomUpdateManager.customUpdateMonos.Add(this);
        InventoryUpdate();
    }
    private void OnDisable()
    {
        CustomUpdateManager.customUpdateMonos.Remove(this);   
    }
    public void CustomUpdate()
    {
        TabEffect();
        SelectTabOn();
        
    }

    public void InventoryUpdate()
    {
        GameMoneyUI_Visualize();
        GameMoneySlot_Visualize();
        EquipmentSlot_Visualize();
    }
    private void TabEffect()
    {
        if(effects.Count > 0 && selectedEffect == null)
        {
            selectedEffect = effects[0];
        }

        for(int i = 0; i < effects.Count; i++)
        {
            if (effects[i] == selectedEffect)
            {
                effects[i].SetActive(true);
            }
            else
            {
                effects[i].SetActive(false);
            }
        }
    }
    private void SelectTabOn()
    {
        int index = (int)currentShowTab;
        for(int i = 0; i < inventoryScrolls.Count; i++)
        {
            if (i == index) 
            {
                if (inventoryScrolls[i].activeSelf != true)
                {
                    inventoryScrolls[i].SetActive(true);
                }
            }
            else
            {
                if (inventoryScrolls[i].activeSelf != false)
                {
                    inventoryScrolls[i].SetActive(false);
                }
            }
        }
    }
    private void GameMoneyUI_Visualize()
    {
        for(int i = 0; i < moneyUIs.Count; i++)
        {
            switch(i)
            {
                case (int)GameMoneyTab.GOLD_UI:
                    moneyUIs[i].ui_Text.text = GameDataBase.instance.playerInfo.gold.ToString("F0");
                    break;
                case (int)GameMoneyTab.RUBY_UI:
                    moneyUIs[i].ui_Text.text = GameDataBase.instance.playerInfo.ruby.ToString("F0");
                    break;
            }
        }
    }
    protected void GameMoneySlot_Visualize()
    {
        Slot_AllClear(InventoryTab.GAMEMONEY_TAB); 

        for(int i = 0; i < GameDataBase.instance.playerInfo.gameMoneyInventory.Count; i++)
        {
            GameMoneyItem item = GameDataBase.instance.playerInfo.gameMoneyInventory[i];
            gameMoneySlots[i].SlotSetting(item);
        }
    }
    
    protected void EquipmentSlot_Visualize()
    {
         Slot_AllClear(InventoryTab.EQUIPMENT_TAB);

        for (int i = 0; i < GameDataBase.instance.playerInfo.equipmentInventory.Count; i++)
        {
            EquipmentItem item = GameDataBase.instance.playerInfo.equipmentInventory[i];
            equipmentSlots[i].SlotSetting(item);
        }
    }

    //모든 슬롯 초기화
    protected void Slot_AllClear(InventoryTab tabName)
    {
        switch (tabName)
        {
            case InventoryTab.GAMEMONEY_TAB:
                for (int i = 0; i < gameMoneySlots.Count; i++)
                {
                    gameMoneySlots[i].inventoryUI = this;
                    gameMoneySlots[i].SlotClear();
                }
                break;
            case InventoryTab.EQUIPMENT_TAB:
                for (int i = 0; i < equipmentSlots.Count; i++)
                {
                    equipmentSlots[i].inventoryUI = this;
                    equipmentSlots[i].SlotClear();
                }
                break;
        
        }
    }

    
    public void SortGrade()
    {
        GameDataBase.instance.playerInfo.equipmentInventory.SortByGrade(isSortByAscending);    
    }
    public void SortLevel_Descend()
    {
        GameDataBase.instance.playerInfo.equipmentInventory.SortByLevel(isSortByAscending);
    }
    public void IsAscending()
    {
        isSortByAscending = true;
    }
    public void IsDescending()
    {
        isSortByAscending = false;
    }


    public void CloseButton()
    { 
        //PoolManager.instance.FalsedPrefab(gameObject, gameObject.name);
    }
}
