using GDBA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUsableUI : InventoryUsableUI<EquipmentItem>
{
    public override void UsableUISetting(EquipmentItem itemData, Sprite iconSprite)
    {
        base.UsableUISetting(itemData, iconSprite);
        if (this.itemData != null)
        {
            itemLevel.text = "LV." + this.itemData.level;
        }
    }

    public override void EquipItem()
    {
        //if(itemData == null)
        //{
        //    return;
        //}

        //PlayerInfo playerInfo = GameDataBase.instance.playerInfo;
        //bool isSuccess = false;

        //playerInfo.equipmentInventory.EquipmentisEquipReset(itemData.type);
        //switch (itemData.type)
        //{
        //    case EquipmentItem.EquipmentType.WEAPON:
        //        playerInfo.weaponData = itemData;
        //        itemData.isEquip = true;
        //        break;
        //    case EquipmentItem.EquipmentType.HELMET:
        //        playerInfo.helmetData = itemData;
        //        itemData.isEquip = true;
        //        break;
        //    case EquipmentItem.EquipmentType.CHEST:
        //        playerInfo.chestData = itemData;
        //        itemData.isEquip = true;
        //        break;
        //    case EquipmentItem.EquipmentType.PANTS:
        //        playerInfo.pantsData = itemData;
        //        itemData.isEquip = true;
        //        break;
        //    case EquipmentItem.EquipmentType.SHOES:
        //        playerInfo.shoesData = itemData;
        //        itemData.isEquip = true;
        //        break;
        //}

        //if (itemData.isEquip == true)
        //{
        //    isSuccess = true;
        //    if(isSuccess == true)
        //    {
        //        Debug.Log("장비 장착: " + itemData.type);
        //    }
        //}

        //inventoryUI.InventoryUpdate();
    }

    public override void UnEquipItem()
    {
        //if (itemData == null)
        //{
        //    return;
        //}

        //PlayerInfo playerInfo = GameDataBase.instance.playerInfo;
        //bool isSuccess = false;
        //switch (itemData.type)
        //{
        //    case EquipmentItem.EquipmentType.WEAPON:
        //        if(playerInfo.weaponData == itemData)
        //        {
        //            playerInfo.weaponData.isEquip = false;
        //            playerInfo.weaponData = null;
        //            itemData.isEquip = false;
        //            isSuccess = true;
        //        }
        //        break;
        //    case EquipmentItem.EquipmentType.HELMET:
        //        if (playerInfo.helmetData == itemData)
        //        {
        //            playerInfo.helmetData.isEquip = false;
        //            playerInfo.helmetData = null;
        //            itemData.isEquip = false;
        //            isSuccess = true;
        //        }
        //        break;
        //    case EquipmentItem.EquipmentType.CHEST:
        //        if (playerInfo.chestData == itemData)
        //        {
        //            playerInfo.chestData.isEquip = false;
        //            playerInfo.chestData = null;
        //            itemData.isEquip = false;
        //            isSuccess = true;
        //        }
        //        break;
        //    case EquipmentItem.EquipmentType.PANTS:
        //        if (playerInfo.pantsData == itemData)
        //        {
        //            playerInfo.pantsData.isEquip = false;
        //            playerInfo.pantsData = null;
        //            itemData.isEquip = false;
        //            isSuccess = true;
        //        }
        //        break;
        //    case EquipmentItem.EquipmentType.SHOES:
        //        if (playerInfo.shoesData == itemData)
        //        {
        //            playerInfo.shoesData.isEquip = false;
        //            playerInfo.shoesData = null;
        //            itemData.isEquip = false;
        //            isSuccess = true;
        //        }
        //        break;
    //}


    //if (isSuccess == true)
    //{
    //    Debug.Log("장비 해제: " + itemData.type);
    //}
    //inventoryUI.InventoryUpdate();
}

    public override void SellItem()
    {
        //if (itemData == null)
        //{
        //    return;
        //}

        //GameDataBase.instance.playerInfo.equipmentInventory.Remove(itemData);
        //EquipmentDataClear(itemData);
        //EquipmentUIClear();
        //inventoryUI.InventoryUpdate();
    }

    private void EquipmentDataClear(EquipmentItem itemData)
    {
        //PlayerInfo playerInfo = GameDataBase.instance.playerInfo;
        //switch(itemData.type)
        //{
        //    case EquipmentItem.EquipmentType.WEAPON:
        //        if (itemData == playerInfo.weaponData)
        //        {
        //            playerInfo.weaponData = null;
        //        }
        //        break;
        //    case EquipmentItem.EquipmentType.HELMET:
        //        if (itemData == playerInfo.helmetData)
        //        {
        //            playerInfo.helmetData = null;
        //        }
        //        break;
        //    case EquipmentItem.EquipmentType.CHEST:
        //        if (itemData == playerInfo.chestData)
        //        {
        //            playerInfo.chestData = null;
        //        }
        //        break;
        //    case EquipmentItem.EquipmentType.PANTS:
        //        if (itemData == playerInfo.pantsData)
        //        {
        //            playerInfo.pantsData = null;
        //        }
        //        break;
        //    case EquipmentItem.EquipmentType.SHOES:
        //        if (itemData == playerInfo.shoesData)
        //        {
        //            playerInfo.shoesData = null;
        //        }
        //        break;
        //}
    } 
    private void EquipmentUIClear()
    {
        itemData = null;
        itemIcon.enabled = false;
        itemLevel.text = "";
    }
}
