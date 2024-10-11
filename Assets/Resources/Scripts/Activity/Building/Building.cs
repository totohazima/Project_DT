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
    [Header("Bool")]
    public bool isInteraction = false; //true일시 상호작용 불가
    private bool onScanning = false;
    [Header("Customer")]
    public List<Collider> scanHero = new List<Collider>();
    public List<HeroCharacter> customerList = new List<HeroCharacter>();
    public Transform interactionCenter = null;
    public Vector3 interactionRange = new Vector3(1f, 1f, 1f);
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
            Collider[] colliders = Physics.OverlapBox(myObject.position, interactionRange, Quaternion.identity, 1 << 8);
            // 겹친 콜라이더에 대해 처리
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

        customer.popupController.Builng_Use(this, buildingDelay);
    }

    //리스트로 받은 아이템 정보를 텍스트로 출력 추후 작업
    public void RewardPopup()
    {
        GameObject text = PoolManager.instance.Spawn(rewardText, interactionCenter.position, Vector3.one, Quaternion.identity, true, FieldManager.instance.spawnPool);

        BuildingRewardText floatText = text.GetComponent<BuildingRewardText>();
        floatText.TextSetting(GameMoney.GameMoneyType.GOLD, 1);
    }


#if UNITY_EDITOR
    int segments = 100;
    bool drawWhenSelected = true;

    void OnDrawGizmosSelected()
    {
        if (drawWhenSelected)
        {
            //탐지 시야
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(interactionCenter.position, interactionRange);
        }
    }
#endif
}
