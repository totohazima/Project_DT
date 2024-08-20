using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTab : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private InventoryUI.InventoryTab tab;
    [SerializeField] private GameObject selectEffect;

    private void Awake()
    {
        inventoryUI.effects.Add(selectEffect);
    }
    public void ClickTab()
    {
        inventoryUI.currentShowTab = tab;
        inventoryUI.selectedEffect = selectEffect;
    }
}
