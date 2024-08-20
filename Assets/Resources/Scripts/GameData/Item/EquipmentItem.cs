using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentItem : GameItem
{
    public enum EquipmentType
    {
        WEAPON = 0,
        HELMET = 1,
        CHEST = 2,
        PANTS = 3,
        SHOES = 4,
    }

    public enum EquipmentGrade
    {
        COMMON = 0,
        RARE = 1,
        EPIC = 2,
        UNIQUE = 3,
    }

    public EquipmentType type;
    public EquipmentGrade grade;
    public int level;
    public int maxLevel;
    public string spriteCode;
    public string uid;
    public string name;
    //public Sprite sprite;
    public bool isEquip;
    public bool isLock;
    public EquipmentInfoTable.BaseStatus status;
    public EquipmentInfoTable.Data data = null;
}
