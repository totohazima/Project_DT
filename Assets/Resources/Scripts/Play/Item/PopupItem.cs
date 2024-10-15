using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupItem : MonoBehaviour
{
    public TextMeshPro text;
    public SpriteRenderer itemIcon;
    public GameObjectEffect effect;
    [Header("ItemInfo")]
    public GameMoney.GameMoneyType itemType;
    public int itemCount = 0;
    [SerializeField] private List<Sprite> itemSprite = new List<Sprite>();

    public void ItemSetting(GameMoney.GameMoneyType type, int count)
    {
        Sprite typeImage = null;
        typeImage = itemSprite[(int)type];
        itemIcon.sprite = typeImage;

        itemCount = count;
        text.text = "+" + itemCount;
        effect.target = gameObject;

        effect.Effect();
    }


    protected void Recycle()
    {
        itemIcon.sprite = itemSprite[(int)itemType];
    }
}
