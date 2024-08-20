using GDBA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class EquipmentSlot : MasterSlot<EquipmentItem>
{
    public Text gradeText;
    public Image equipMark;
    public override void SlotSetting(EquipmentItem itemType)
    {
        base.SlotSetting(itemType);
    }
    public override void SlotSpriteSetting(EquipmentItem itemType)
    {
        backGroundImage.sprite = tierSprite[(int)itemType.grade];

        Sprite itemSprite = null;
        switch (item.type)
        {
            case EquipmentItem.EquipmentType.WEAPON:
                itemSprite = GameDataBase.instance.weaponSpriteAtlas.GetSprite(item.data.spriteCode);
                break;
            case EquipmentItem.EquipmentType.HELMET:
                itemSprite = GameDataBase.instance.helmetSpriteAtlas.GetSprite(item.data.spriteCode);
                break;
            case EquipmentItem.EquipmentType.CHEST:
                itemSprite = GameDataBase.instance.weaponSpriteAtlas.GetSprite(item.data.spriteCode);
                break;
            case EquipmentItem.EquipmentType.PANTS:
                itemSprite = GameDataBase.instance.weaponSpriteAtlas.GetSprite(item.data.spriteCode);
                break;
            case EquipmentItem.EquipmentType.SHOES:
                itemSprite = GameDataBase.instance.weaponSpriteAtlas.GetSprite(item.data.spriteCode);
                break;
        }
        icon.enabled = true;
        icon.sprite = itemSprite;
    }
    public override void SlotEffectSetting(EquipmentItem itemType)
    {
        string text = null;
        switch(item.grade)
        {
            case EquipmentItem.EquipmentGrade.COMMON:
                text = "Common";
                break;
            case EquipmentItem.EquipmentGrade.RARE:
                text = "Rare";
                break;
            case EquipmentItem.EquipmentGrade.EPIC:
                text = "Epic";
                break;
            case EquipmentItem.EquipmentGrade.UNIQUE:
                text = "Unique";
                break;
            default:
                text = "";
                break;
        }
        gradeText.text = text;

        if(item.isEquip == true)
        {
            equipMark.enabled = true;
        }
        else
        {
            equipMark.enabled = false;
        }
    }
    public override void SlotClear()
    {
        base.SlotClear();
        gradeText.text = "";
        equipMark.enabled = false;
    }

    public override void ClickUIPopup()
    {
        inventoryUI.equipmentUsableUI.UsableUISetting(item, icon.sprite);   
    }
}
