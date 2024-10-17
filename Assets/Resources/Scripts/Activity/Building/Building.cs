using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : FieldObject
{
    public enum BuildingType
    {
        NONE = 0,
        TRADE = 1,
        MEDICAL = 2,
        BAR = 3,
        RESTAURANT = 4,
        INN = 5,
    }

    public Collider2D myCollider2D;
    [Header("Status")]
    public BuildingType buildingType;
    public string buildingName;
    public int builngLevel = 1;
    public float buildingDelay;
    public GameMoney.GameMoneyType giveItenType;
    [Header("Bool")]
    public bool isInteraction = false; //true�Ͻ� ��ȣ�ۿ� �Ұ�
    private bool onScanning = false;
    [Header("Customer")]
    public List<Collider> scanHero = new List<Collider>();
    public List<HeroCharacter> customerList = new List<HeroCharacter>();
    public Transform interactionCenter = null;
    public Vector3 interactionRange = new Vector3(1f, 1f, 1f);
    public Transform popupTransform = null;
    private GameObject rewardText;
    private void Awake()
    {
        rewardText = Resources.Load<GameObject>("Prefabs/FieldObject/BuildingRewardTxt");

    }
    private void Update()
    {
        StartCoroutine(characterScan());

        foreach(HeroCharacter waitingUnit in customerList)
        {
            waitingUnit.isWaitingBuilding = true;
        }

        if(customerList.Count > 0 && !isInteraction)
        {
            Interaction(customerList[0]);
        }
    }

    private IEnumerator characterScan()
    {
        if (!onScanning)
        {
            onScanning = true;

            scanHero.Clear();
            Collider[] colliders = Physics.OverlapBox(interactionCenter.position, interactionRange / 2, Quaternion.identity, 1 << 8);
            // ��ģ �ݶ��̴��� ���� ó��
            foreach (Collider hitCollider in colliders)
            {
                scanHero.Add(hitCollider);
            }

            foreach(Collider hero in scanHero)
            {
                foreach(HeroCharacter character in FieldManager.instance.heroList)
                {
                    if(hero == character.myCollider && character.targetBuilding == buildingType && !customerList.Contains(character))
                    {
                        customerList.Add(character);
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
            onScanning = false;
        }
    }

    private void Interaction(HeroCharacter customer)
    {
        isInteraction = true;

        StartCoroutine(customer.popupController.Building_Interaction(this, buildingDelay));
    }

    //����Ʈ�� ���� ������ ������ �ؽ�Ʈ�� ���
    public void RewardPopup(GameMoney.GameMoneyType type, int count)
    {
        GameObject text = PoolManager.instance.Spawn(rewardText, interactionCenter.position, Vector3.one, Quaternion.identity, true, PoolManager.instance.spawnRoot);
        text.transform.position = popupTransform.position;

        BuildingRewardText floatText = text.GetComponent<BuildingRewardText>();
        floatText.TextSetting(type, count, popupTransform.position);
    }

    /// <summary>
    /// �ǹ� ��ȣ�ۿ� �ִϸ��̼�
    /// </summary>
    public IEnumerator Building_Interaction(HeroCharacter hero, float useDelay)
    {
        float giveDelay = 0.2f;
        int count = 1;
        for (int i = 0; i < count; i++)
        {
            AddGiveBuildingItem(giveItenType, Random.Range(1, 1), hero);
            yield return new WaitForSeconds(giveDelay);
        }

    }
    protected void AddGiveBuildingItem(GameMoney.GameMoneyType type, int count, HeroCharacter hero)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/FieldObject/GiveItem");
        GameObject popup = PoolManager.instance.Spawn(prefab, hero.popupCenter.position, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, true, PoolManager.instance.spawnRoot);

        GiveItem popupItem = popup.GetComponent<GiveItem>();
        popupItem.character = hero;
        popupItem.ItemSetting(type, count, popupTransform.position);
    }

#if UNITY_EDITOR
    int segments = 100;
    bool drawWhenSelected = true;

    void OnDrawGizmosSelected()
    {
        if (drawWhenSelected)
        {
            //Ž�� �þ�
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(interactionCenter.position, interactionRange);
        }
    }
#endif
}
