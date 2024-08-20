using GDBA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentAddCheat : MonoBehaviour
{
    public void WeaponAdd()
    {
        EquipmentInfoTable infoTable = GameDataBase.instance.weaponInfoTable;
        EquipmentInventory inventory = GameDataBase.instance.playerInfo.equipmentInventory;

        int randomNum = 0;
        randomNum = Random.Range(0, infoTable.Count);
        EquipmentInfoTable.Data data = infoTable.table[randomNum];

        EquipmentItem item = new EquipmentItem();

        item.maxLevel = data.maxLevel;
        item.spriteCode = data.spriteCode;
        item.code = data.code;
        item.name = data.name;
        item.grade = data.grade;
        item.type = EquipmentItem.EquipmentType.WEAPON;
        item.status.statusType = data.baseStatus.statusType;
        item.status.riseType = data.baseStatus.riseType;
        item.status.baseValue = data.baseStatus.baseValue;
        item.status.riseValue = data.baseStatus.riseValue;

        item.isEquip = false;
        item.isLock = false;
        item.data = data;

        inventory.Add(item);

        Debug.Log("무기 추가 완료");
    }

    public void HelmetAdd()
    {
        EquipmentInfoTable infoTable = GameDataBase.instance.helmetInfoTable;
        EquipmentInventory inventory = GameDataBase.instance.playerInfo.equipmentInventory;

        int randomNum = 0;
        randomNum = Random.Range(0, infoTable.Count);
        EquipmentInfoTable.Data data = infoTable.table[randomNum];

        EquipmentItem item = new EquipmentItem();

        item.maxLevel = data.maxLevel;
        item.spriteCode = data.spriteCode;
        item.code = data.code;
        item.name = data.name;
        item.grade = data.grade;
        item.type = EquipmentItem.EquipmentType.HELMET;
        item.status.statusType = data.baseStatus.statusType;
        item.status.riseType = data.baseStatus.riseType;
        item.status.baseValue = data.baseStatus.baseValue;
        item.status.riseValue = data.baseStatus.riseValue;

        item.isEquip = false;
        item.isLock = false;
        item.data = data;

        inventory.Add(item);

        Debug.Log("투구 추가 완료");
    }
}
