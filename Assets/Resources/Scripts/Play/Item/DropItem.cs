using GameSystem;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropItem : FieldObject, IPointerClickHandler
{
    [Header("ItemInfo")]
    public GameMoney.GameMoneyType moneyType;
    public int dropCount = 1;
    private bool isGetItem = false; //true인 경우 습득 가능

    public void Drop_Animation(Vector3 dropPos)
    {
        LTDescr tween = LeanTween.move(gameObject, dropPos, 0.3f).setEase(LeanTweenType.easeInSine);
        tween.setOnComplete(End_Animation);
    }

    void End_Animation()
    {
        isGetItem = true;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isGetItem)
        {
            GetItem();
        }
    }

    protected void GetItem()
    {
        PlayerInfo playerInfo = GameManager.instance.gameDataBase.playerInfo;

        switch(moneyType)
        {
            case GameMoney.GameMoneyType.GOLD:
                playerInfo.gold += dropCount;
                break;
            case GameMoney.GameMoneyType.RUBY:
                playerInfo.ruby += dropCount;
                break;
        }

        ShowFloatingText();
        Disappear();
    }

    protected void ShowFloatingText()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/FieldObject/ItemRewardTxt");
        GameObject text = PoolManager.instance.Spawn(prefab, myObject.position, Vector3.one, Quaternion.identity, true, myObject.parent);
        ItemRewardText floatText = text.GetComponent<ItemRewardText>();
        floatText.TextSetting(moneyType, dropCount);
    }
    protected void Disappear()
    {
        isGetItem = false;
        PoolManager.instance.Release(gameObject);    
    }
}
