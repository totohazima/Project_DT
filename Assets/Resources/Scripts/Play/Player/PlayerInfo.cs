using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDBA;
using StatusHelper;

[CreateAssetMenu(menuName = "GameDataBase/PlayerInfo")]
public class PlayerInfo : ScriptableObject
{
    #region Game Money
    [Header("#Game Money")]
    public double gold;
    public double ruby;
    public double Gold
    {
        get { return gold; }
    }
    public double Ruby
    {
        get { return ruby; }
    }
    #endregion

    #region Inventory
    [Header("#Inventory")]
    public GameMoneyInventory gameMoneyInventory = new GameMoneyInventory();
    public EquipmentInventory equipmentInventory = new EquipmentInventory();
    #endregion

    public void StatCalculate()
    {
        
    }
}
