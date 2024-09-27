using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupItem : FloatingText
{
    public SpriteRenderer itemIcon;
    [Header("ItemInfo")]
    public GameMoney.GameMoneyType itemType;
    public int itemCount = 0;

    [SerializeField] private List<Sprite> itemSprite = new List<Sprite>();

    public void ItemSetting(GameMoney.GameMoneyType type, int count)
    {
        Sprite typeImage = null;
        typeImage = itemSprite[(int)type];

        //itemIcon.sprite = typeImage;
        StartCoroutine(Text_Animation());
    }

    public override IEnumerator Text_Animation()
    {
        Vector3 destination = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        Vector3 fromPos = new Vector3(destination.x, destination.y - 0.3f, destination.z);

        LeanTween.move(gameObject, destination, 1f).setEase(LeanTweenType.easeOutSine).setFrom(fromPos);

        yield return new WaitForSeconds(1f);

        //LeanTween.value(itemIcon.gameObject, 1, 0, 0.2f).setOnUpdate(UpdateTextAlpha);

        //yield return new WaitForSeconds(0.2f);

        Recycle();
        Disappeer();
    }

    protected override void Recycle()
    {
        //itemIcon.sprite = null;
    }
    private void UpdateTextAlpha(float alpha)
    {
        Color color = itemIcon.color;
        color.a = alpha;
        itemIcon.color = color;
    }

    public override void Disappeer()
    {
        base.Disappeer();
    }
}
