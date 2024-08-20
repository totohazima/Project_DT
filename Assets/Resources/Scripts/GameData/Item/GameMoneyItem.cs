using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDBA;

[System.Serializable]
public class GameMoneyItem : GameItem
{
    public enum GameMoneyType
    {
        GOLD = 0,
        RUBY = 1,
    }

    public GameMoneyType type;
    public double amount;

    public GameMoneyItem(GameMoneyType _type, double _amount)
    {
        type = _type;
        amount = _amount;
    }
}
