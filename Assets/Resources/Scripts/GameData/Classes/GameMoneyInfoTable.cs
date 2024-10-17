using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDBA;

public class GameMoneyInfoTable : GameDataTable<GameMoneyInfoTable.Data> 
{
    [System.Serializable]
    public class Data : GameData
    {
        public string num;
        public string gameMoneyCode;
        public string gameMoneyName;
        public string spriteCode;
    }
}
