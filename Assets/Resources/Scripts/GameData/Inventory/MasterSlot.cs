using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MasterSlot<T> : MonoBehaviour
{
    [HideInInspector] public T item;
    public InventoryUI inventoryUI;
    public Image icon;
    public Text itemCount;
    public Image backGroundImage;
    public Sprite[] tierSprite;

    public virtual void SlotSetting(T itemType)
    {
        if (itemType != null)
        {
            item = itemType;
            SlotSpriteSetting(item);
            SlotEffectSetting(item);
        }
    }

    public virtual void SlotSpriteSetting(T itemType)
    { }

    public virtual void SlotEffectSetting(T itemType)
    { }
    public virtual void SlotClear()
    {
        item = default;
        icon.sprite = null;
        icon.enabled = false;
        itemCount.gameObject.SetActive(false);
        backGroundImage.sprite = tierSprite[0];
    }

    public virtual void ClickUIPopup()
    { }
}
