using GDBA;
using StatusHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInfoTable : GameDataTable<EquipmentInfoTable.Data>
{
    [System.Serializable]
    public struct BaseStatus
    {
        public ESTATUS statusType;
        public ERISE_TYPE riseType;
        public double baseValue;
        public double riseValue;
    }

    [System.Serializable]
    public class Data : GameData
    {
        public string spriteCode;
        public string code;
        public string name;
        public EquipmentItem.EquipmentGrade grade;
        public BaseStatus baseStatus;
        public int maxLevel;
    }
}
