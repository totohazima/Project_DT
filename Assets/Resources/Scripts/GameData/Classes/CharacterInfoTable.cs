using GDBA;
using StatusHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfoTable : GameDataTable<CharacterInfoTable.Data>
{
    [System.Serializable]
    public class Data : GameData
    {
        public string characterCode;
        public string characterName;
        public string characterClass;
        public string characterCostumeCode;
    }
}
