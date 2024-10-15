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
        GameObject popup = PoolManager.instance.Spawn(prefab, hero.popupPos.position, Vector3.one, Quaternion.identity, true, gameObject.transform);

        PopupItem popupItem = popup.GetComponent<PopupItem>();
        popupItem.ItemSetting(type, count);
    }


    /// <summary>
    /// 건물 상호작용 애니메이션
    /// </summary>
    public IEnumerator Building_Interaction(Building building, float useDelay)
    {
        GiveBuildingItem(GameManager.instance.GetRandomEnumValue<GameMoney.GameMoneyType>(0), Random.Range(1, 4), building);

        yield return new WaitForSeconds(useDelay);

        hero.targetBuilding = Building.BuildingType.NONE;
        hero.isWaitingBuilding = false;
        building.isInteraction = false;
        building.customerList.Remove(hero);
    }
    protected void GiveBuildingItem(GameMoney.GameMoneyType type, int count, Building building)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/FieldObject/GiveItem");
        GameObject popup = PoolManager.instance.Spawn(prefab, hero.popupPos.position, Vector3.one, Quaternion.identity, true, gameObject.transform);

        GiveBuildingItem popupItem = popup.GetComponent<GiveBuildingItem>();
        popupItem.target = building;
        popupItem.ItemSetting(type, count);
    }
}
