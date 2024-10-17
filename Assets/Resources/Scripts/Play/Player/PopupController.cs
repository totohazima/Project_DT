using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    public HeroCharacter hero;

    public void ItemPopup(List<GameMoney.GameMoneyType> type, List<int> count)
    {
        for(int i = 0; i < type.Count; i++)
        {
            PopupItem(type[i], count[i]);
        }
    }

    protected void PopupItem(GameMoney.GameMoneyType type, int count)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/FieldObject/PopupItem");
        GameObject popup = PoolManager.instance.Spawn(prefab, hero.popupCenter.position, Vector3.one, Quaternion.identity, true, gameObject.transform);

        PopupItem popupItem = popup.GetComponent<PopupItem>();
        popupItem.ItemSetting(type, count);
    }


    /// <summary>
    /// 건물 상호작용 애니메이션
    /// </summary>
    public IEnumerator Building_Interaction(Building building, float useDelay)
    {
        float giveDelay = 0.2f;
        int count = 5;
        for (int i = 0; i < count; i++)
        {
            AddGiveUnitItem(GameManager.instance.GetRandomEnumValue<GameMoney.GameMoneyType>(0, 4), Random.Range(1, 4), building);
            yield return new WaitForSeconds(giveDelay);
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(building.Building_Interaction(hero, useDelay)); 
    }
    protected void AddGiveUnitItem(GameMoney.GameMoneyType type, int count, Building building)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/FieldObject/GiveItem");
        GameObject popup = PoolManager.instance.Spawn(prefab, hero.popupCenter.position, Vector3.one, Quaternion.identity, true, hero.myObject.parent);

        GiveItem popupItem = popup.GetComponent<GiveItem>();
        popupItem.building = building;
        popupItem.ItemSetting(type, count, transform.position);
    }

    public void EndInteraction()
    {
        Building build = BuildingManager.Instance.buildings[(int)hero.targetBuilding - 1];

        hero.targetBuilding = Building.BuildingType.NONE;
        hero.isWaitingBuilding = false;
        build.isInteraction = false;
        build.customerList.Remove(hero);
    }
}
