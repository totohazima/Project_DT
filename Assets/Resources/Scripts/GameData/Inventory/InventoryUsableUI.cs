using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUsableUI<T> : MonoBehaviour
{
    public T itemData;
    public Image itemIcon;
    public Text itemLevel;
    public InventoryUI inventoryUI;
    public virtual void UsableUISetting(T itemData, Sprite iconSprite)
    {
        this.itemData = itemData;
        itemIcon.enabled = true;
        itemIcon.sprite = iconSprite;
    }

    public virtual void EquipItem()
    { }

    public virtual void UnEquipItem()
    { }

    public virtual void SellItem()
    { }
}
