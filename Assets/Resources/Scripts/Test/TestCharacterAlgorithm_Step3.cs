using FieldHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterAlgorithm_Step3 : MonoBehaviour
{
    public HeroCharacter character;
    public FieldMap.Field combatField;
    public float combatTime = 0f;
    private float combatTimer = 0f;

    private void Start()
    {
        character = GetComponent<HeroCharacter>();
        
        StartCoroutine(GoCombatField(combatField));
    }
    private void Update()
    {
        if(character.isReadyToAttack)
        {
            combatTimer -= Time.deltaTime;  
        }

        if (combatTimer <= 0f && !character.isReadyToAttack || FieldManager.instance.fields[(int)combatField].monsters.Count == 0)
        {
            StartCoroutine(GoVillage());
        }
    }

    private IEnumerator GoCombatField(FieldMap.Field combatField)
    {
        character.targetField = FieldManager.instance.fieldList[(int)combatField];
        character.targetUnit = null;
        combatTimer = combatTime;

        while (true)
        {
            if (character.myField == combatField)
            {
                yield return new WaitForSeconds(3f);

                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator GoVillage()
    {
        character.targetField = FieldManager.instance.fieldList[(int)FieldMap.Field.VILLAGE];
        character.targetUnit = null;
        while(true)
        {
            character.isStopScanning = true;

            if(character.myField == FieldMap.Field.VILLAGE)
            {
                yield return new WaitForSeconds(10f);
                StartCoroutine(GoCombatField(combatField));
                yield break;
            }

            yield return null;
        }
    }
}
