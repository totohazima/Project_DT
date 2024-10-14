using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    public HeroCharacter hero;

    /// <summary>
    /// �ǹ� ��ȣ�ۿ� �ִϸ��̼�
    /// </summary>
    public void Builng_Use(Building building, float useDelay)
    {
        StartCoroutine(Building_Interaction(building, useDelay));
    }
    protected IEnumerator Building_Interaction(Building building, float useDelay)
    {
        //Debug.Log(characterName + building.buildingName + " ��ȣ�ۿ� ����");
        ItemPopup(GameManager.instance.GetRandomEnumValue<GameMoney.GameMoneyType>(0), Random.Range(1, 4), building);

        yield return new WaitForSeconds(useDelay);

        hero.targetBuilding = Building.BuildingType.NONE;
        hero.isWaitingBuilding = false;
        building.isInteraction = false;
        building.customerList.Remove(hero);
        //Debug.Log(characterName + building.buildingName + " ��ȣ�ۿ� �Ϸ�");
    }

    protected void ItemPopup(GameMoney.GameMoneyType type, int count, Building building)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/FieldObject/PopupItem");
        GameObject popup = PoolManager.instance.Spawn(prefab, hero.popupPos.position, Vector3.one, Quaternion.identity, true, hero.myObject.parent);

        PopupItem popupItem = popup.GetComponent<PopupItem>();
        popupItem.target = building;
        popupItem.ItemSetting(type, count);
    }
}
