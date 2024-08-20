//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using GameInventory;
//using UnityEditor.Search;
//using Unity.VisualScripting;
//public class EquipmentInventory : MasterInventory<EquipmentItem>
//{
//    public override void Add(EquipmentItem item)
//    {
//        base.Add(item);
//    }

//    public void EquipmentisEquipReset(EquipmentItem.EquipmentType type)
//    {
//        foreach (EquipmentItem item in inventory)
//        {
//            if (item.type == type)
//            {
//                item.isEquip = false;
//            }
//        }
//    }

//    //장비타입에 따라 분류
//    private (List<EquipmentItem>, List<EquipmentItem>, List<EquipmentItem>, List<EquipmentItem>, List<EquipmentItem>, List<EquipmentItem>) SortByType()
//    {
//        List<EquipmentItem> equipList = new List<EquipmentItem>();
//        List<EquipmentItem> weaponList = new List<EquipmentItem>();
//        List<EquipmentItem> helmetList = new List<EquipmentItem>();
//        List<EquipmentItem> chestList = new List<EquipmentItem>();
//        List<EquipmentItem> pantsList = new List<EquipmentItem>();
//        List<EquipmentItem> shoesList = new List<EquipmentItem>();

//        foreach (EquipmentItem item in inventory)
//        {
//            if (item.isEquip == true)
//            {
//                equipList.Add(item);
//            }
//            else
//            {
//                switch(item.type)
//                {
//                    case EquipmentItem.EquipmentType.WEAPON:
//                        weaponList.Add(item);
//                        break;
//                    case EquipmentItem.EquipmentType.HELMET:
//                        helmetList.Add(item);
//                        break;
//                    case EquipmentItem.EquipmentType.CHEST:
//                        chestList.Add(item);
//                        break;
//                    case EquipmentItem.EquipmentType.PANTS:
//                        pantsList.Add(item);
//                        break;
//                    case EquipmentItem.EquipmentType.SHOES:
//                        shoesList.Add(item);
//                        break;
//                }
//            }
//        }

//        equipList.Sort((x, y) => x.type.CompareTo(x.type));

//        return (equipList, weaponList, helmetList, chestList, pantsList, shoesList);
//    }

//    /// <summary>
//    /// 등급별 정렬
//    /// </summary>
//    /// <param name="isAscendingOrder">true일 경우 오름차순</param>
//    public void SortByGrade(bool isAscendingOrder)
//    {
//        List<EquipmentItem> totalList = new List<EquipmentItem>();
//        List<EquipmentItem> equipList = new List<EquipmentItem>();
//        List<EquipmentItem> weaponList = new List<EquipmentItem>();
//        List<EquipmentItem> helmetList = new List<EquipmentItem>();
//        List<EquipmentItem> chestList = new List<EquipmentItem>();
//        List<EquipmentItem> pantsList = new List<EquipmentItem>();
//        List<EquipmentItem> shoesList = new List<EquipmentItem>();

//        (equipList, weaponList, helmetList, chestList, pantsList, shoesList) = SortByType();

//        SortByGrader(weaponList, isAscendingOrder);
//        SortByGrader(helmetList, isAscendingOrder);
//        SortByGrader(chestList, isAscendingOrder);
//        SortByGrader(pantsList, isAscendingOrder);
//        SortByGrader(shoesList, isAscendingOrder);

//        equipList.AddRange(weaponList);
//        equipList.AddRange(helmetList);
//        equipList.AddRange(chestList);
//        equipList.AddRange(pantsList);
//        equipList.AddRange(shoesList);
//        totalList = equipList;

//        inventory = totalList;
//    }

//    protected List<EquipmentItem> SortByGrader(List<EquipmentItem> list, bool isAscendingOrder)
//    {
//        if (isAscendingOrder)
//        {
//            list.Sort((x, y) =>
//            {
//                int result = x.grade.CompareTo(y.grade);
//                if (result == 0)
//                {
//                    result = x.code.CompareTo(y.code);

//                }
//                return result;
//            });
//        }
//        else
//        {
//            list.Sort((x, y) =>
//            {
//                int result = y.grade.CompareTo(x.grade);
//                if (result == 0)
//                {
//                    result = x.code.CompareTo(y.code);

//                }
//                return result;
//            });
//        }

//        return list;
//    }
//    /// <summary>
//    /// 레벨별 정렬
//    /// </summary>
//    /// <param name="isAscendingOrder">true일 경우 오름차순</param>
//    public void SortByLevel(bool isAscendingOrder)
//    {
//        List<EquipmentItem> totalList = new List<EquipmentItem>();
//        List<EquipmentItem> equipList = new List<EquipmentItem>();
//        List<EquipmentItem> weaponList = new List<EquipmentItem>();
//        List<EquipmentItem> helmetList = new List<EquipmentItem>();
//        List<EquipmentItem> chestList = new List<EquipmentItem>();
//        List<EquipmentItem> pantsList = new List<EquipmentItem>();
//        List<EquipmentItem> shoesList = new List<EquipmentItem>();

//        (equipList, weaponList, helmetList, chestList, pantsList, shoesList) = SortByType();

//        SortByLeveler(weaponList, isAscendingOrder);
//        SortByLeveler(helmetList, isAscendingOrder);
//        SortByLeveler(chestList, isAscendingOrder);
//        SortByLeveler(pantsList, isAscendingOrder);
//        SortByLeveler(shoesList, isAscendingOrder);

//        equipList.AddRange(weaponList);
//        equipList.AddRange(helmetList);
//        equipList.AddRange(chestList);
//        equipList.AddRange(pantsList);
//        equipList.AddRange(shoesList);
//        totalList = equipList;

//        inventory = totalList;
//    }

//    protected List<EquipmentItem> SortByLeveler(List<EquipmentItem> list, bool isAscendingOrder)
//    {
//        if (isAscendingOrder)
//        {
//            list.Sort((x, y) =>
//            {
//                int result = x.level.CompareTo(y.level);
//                if (result == 0)
//                {
//                    result = x.code.CompareTo(y.code);
//                }
//                return result;
//            });
//        }
//        else
//        {
//            list.Sort((x, y) =>
//            {
//                int result = y.level.CompareTo(x.level);
//                if (result == 0)
//                {
//                    result = x.code.CompareTo(y.code);
//                }
//                return result;
//            });
//        }

//        return list;
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInventory;
using System;
using Unity.VisualScripting;

public class EquipmentInventory : MasterInventory<EquipmentItem>
{
    enum ListName
    {
        EQUIP_LIST = 0,
        WEAPON_LIST = 1,
        HELMET_LIST = 2,
        CHEST_LIST = 3,
        PANTS_LIST = 4,
        SHOES_LIST = 5,
    }

    private readonly string[] listName = Enum.GetNames(typeof(ListName));
    public override void Add(EquipmentItem item)
    {
        base.Add(item);
    }

    public void EquipmentisEquipReset(EquipmentItem.EquipmentType type)
    {
        foreach (EquipmentItem item in inventory)
        {
            if (item.type == type)
            {
                item.isEquip = false;
            }
        }
    }

    // 장비 타입별 분류
    private Dictionary<string, List<EquipmentItem>> SortByType()
    {
        var dict = new Dictionary<string, List<EquipmentItem>>()
        {
            { listName[(int)ListName.EQUIP_LIST], new List<EquipmentItem>() },
            { listName[(int)ListName.WEAPON_LIST], new List<EquipmentItem>() },
            { listName[(int)ListName.HELMET_LIST], new List<EquipmentItem>() },
            { listName[(int)ListName.CHEST_LIST], new List<EquipmentItem>() },
            { listName[(int)ListName.PANTS_LIST], new List<EquipmentItem>() },
            { listName[(int) ListName.SHOES_LIST], new List<EquipmentItem>() }
        };

        foreach (EquipmentItem item in inventory)
        {
            if (item.isEquip)
            {
                dict[listName[(int)ListName.EQUIP_LIST]].Add(item);
            }
            else
            {
                switch (item.type)
                {
                    case EquipmentItem.EquipmentType.WEAPON:
                        dict[listName[(int)ListName.WEAPON_LIST]].Add(item);
                        break;
                    case EquipmentItem.EquipmentType.HELMET:
                        dict[listName[(int)ListName.HELMET_LIST]].Add(item);
                        break;
                    case EquipmentItem.EquipmentType.CHEST:
                        dict[listName[(int)ListName.CHEST_LIST]].Add(item);
                        break;
                    case EquipmentItem.EquipmentType.PANTS:
                        dict[listName[(int)ListName.PANTS_LIST]].Add(item);
                        break;
                    case EquipmentItem.EquipmentType.SHOES:
                        dict[listName[(int)ListName.SHOES_LIST]].Add(item);
                        break;
                }
            }
        }

        dict[listName[(int)ListName.EQUIP_LIST]].Sort((x, y) => x.type.CompareTo(y.type));

        return dict;
    }

    /// <summary>
    /// 등급별 정렬
    /// </summary>
    /// <param name="isAscendingOrder">true일 경우 오름차순</param>
    public void SortByGrade(bool isAscendingOrder)
    {
        var dict = SortByType();

        for (int i = 1; i < listName.Length; i++)
        {
            SortByGrader(dict[listName[i]], isAscendingOrder);
        }

        for (int i = 1; i < listName.Length; i++)
        {
            dict[listName[(int)ListName.EQUIP_LIST]].AddRange(dict[listName[i]]);
        }

        inventory = dict[listName[(int)ListName.EQUIP_LIST]];
    }

    protected List<EquipmentItem> SortByGrader(List<EquipmentItem> list, bool isAscendingOrder)
    {
        if (isAscendingOrder)
        {
            list.Sort((x, y) =>
            {
                int result = x.grade.CompareTo(y.grade);
                if (result == 0)
                {
                    result = x.code.CompareTo(y.code);
                }
                return result;
            });
        }
        else
        {
            list.Sort((x, y) =>
            {
                int result = y.grade.CompareTo(x.grade);
                if (result == 0)
                {
                    result = x.code.CompareTo(y.code);
                }
                return result;
            });
        }

        return list;
    }

    /// <summary>
    /// 레벨별 정렬
    /// </summary>
    /// <param name="isAscendingOrder">true일 경우 오름차순</param>
    public void SortByLevel(bool isAscendingOrder)
    {
        var dict = SortByType();

        for (int i = 1; i < listName.Length; i++)
        {
            SortByLeveler(dict[listName[i]], isAscendingOrder);
        }

        for (int i = 1; i < listName.Length; i++)
        {
            dict[listName[(int)ListName.EQUIP_LIST]].AddRange(dict[listName[i]]);
        }

        inventory = dict[listName[(int)ListName.EQUIP_LIST]];
    }

    protected List<EquipmentItem> SortByLeveler(List<EquipmentItem> list, bool isAscendingOrder)
    {
        if (isAscendingOrder)
        {
            list.Sort((x, y) =>
            {
                int result = x.level.CompareTo(y.level);
                if (result == 0)
                {
                    result = x.code.CompareTo(y.code);
                }
                return result;
            });
        }
        else
        {
            list.Sort((x, y) =>
            {
                int result = y.level.CompareTo(x.level);
                if (result == 0)
                {
                    result = x.code.CompareTo(y.code);
                }
                return result;
            });
        }

        return list;
    }
}
