using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInventory;
public class GameMoneyInventory : MasterInventory<GameMoneyItem>
{
    public override void Add(GameMoneyItem item)
    {
        base.Add(item);
    }
}
