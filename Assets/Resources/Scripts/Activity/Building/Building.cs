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

    [Header("Status")]
    public BuildingType buildingType;
    public string buildingName;
    public int builngLevel = 1;
    [Header("Bool")]
    public bool isInteraction = false; //true일시 상호작용 불가
    private bool onScanning = false;
    [Header("Customer")]
    public List<Collider> scanHero = new List<Collider>();
    public List<HeroCharacter> customerList = new List<HeroCharacter>();
    public Transform interactionCenter = null;
    [SerializeField] private Vector3 interactionRange = new Vector3(1f, 1f, 1f);
    private void Update()
    {
        StartCoroutine(characterScan());

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
            Collider[] colliders = Physics.OverlapBox(myObject.position, interactionRange / 2, Quaternion.identity, 1 << 8);
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

        customer.Builng_Use(this);
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
