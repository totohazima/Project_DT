using FieldHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterAlgorithm_Step3 : MonoBehaviour
{
    public HeroCharacter character;
    public FieldMap.Field combatField;
    public float combatTime = 0f;

    private void Start()
    {
        character = GetComponent<HeroCharacter>();

        StartCoroutine(GoCombatField(combatField));
    }
    private void Update()
    {
        if(character.isReadyToAttack)
        {
            combatTime -= Time.deltaTime;  
        }

        if (combatTime <= 0f && !character.isReadyToAttack)
        {
            StartCoroutine(GoVillage());
        }
    }

    private IEnumerator GoCombatField(FieldMap.Field combatField)
    {
        character.targetField = FieldManager.instance.fieldList[(int)combatField];
        character.targetUnit = null;
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
                yield return new WaitForSeconds(3f);
                yield break;
            }

            yield return null;
        }
    }
}
