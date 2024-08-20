using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMoneySlot : MasterSlot<GameMoneyItem>
{
    public override void SlotSetting(GameMoneyItem itemType)
    {
        base.SlotSetting(itemType);
    }

    public override void SlotSpriteSetting(GameMoneyItem itemType)
    {
        backGroundImage.sprite = tierSprite[0];    
    }
    public override void SlotEffectSetting(GameMoneyItem itemType)
    {
            
    }
    public override void SlotClear()
    {
        base.SlotClear();
    }
}
