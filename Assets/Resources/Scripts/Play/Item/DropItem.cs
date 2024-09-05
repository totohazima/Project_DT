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

    public void OnPointerClick(PointerEventData eventData)
    {
        GetItem();    
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
        
        Debug.Log(moneyType.ToString() + dropCount + "Reward");
        Disappear();
    }

    protected void ShowFloatingText()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/FieldObject/ItemRewardTxt");
        ItemRewardText floatText = prefab.GetComponent<ItemRewardText>();
        floatText.parentTransform = myObject;
        floatText.TextSetting(moneyType, dropCount);

        GameObject text = PoolManager.instance.Spawn(floatText.gameObject, myObject.position, Vector3.one, Quaternion.identity, true, myObject.parent);
    }
    protected void Disappear()
    {
        PoolManager.instance.Release(gameObject);    
    }
}
