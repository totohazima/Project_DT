using FieldHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAlgorithm : MonoBehaviour
{
    public HeroCharacter character;
    public FieldMap.Field targetField;
    public float combatTime = 0f;
    [SerializeField] private float combatTimer = 0f;
    private bool afterBuildingUse = false;

    private void Awake()
    {
        character = GetComponent<HeroCharacter>();

        Recycle();
    }

    private void Update()
    {
        CombatTimeUpdate();  
        
        StartCombatField();
        StartVillage();
    }

    private void CombatTimeUpdate()
    {
        if (!character.isReadyToAttack)
        {
            return; 
        }

        combatTimer -= Time.deltaTime;
    }

    private void StartCombatField()
    {
        if (combatTimer <= 0f || character.myField == targetField)
        {
            return;    
        }

        character.targetField = targetField;
    }

    private void StartVillage()
    {
        if(combatTimer > 0f || character.isReadyToAttack)
        {
            return;
        }

        character.targetField = FieldMap.Field.VILLAGE;
        character.isStopScanning = true;

        StartCoroutine(StartBuilding());
    }

    private IEnumerator StartBuilding()
    {
        if(combatTimer > 0f || character.myField != FieldMap.Field.VILLAGE)
        {
            yield break;
        }

        if (character.targetBuilding == Building.BuildingType.NONE && !afterBuildingUse)
        {
            afterBuildingUse = true;
            character.targetBuilding = GameManager.instance.GetRandomEnumValue<Building.BuildingType>(1);
        }

        while (character.targetBuilding != Building.BuildingType.NONE)
        {
            yield return null;
        }

        Recycle();
    }
    private void Recycle()
    {
        combatTimer = combatTime;
        afterBuildingUse = false;
    }
}
