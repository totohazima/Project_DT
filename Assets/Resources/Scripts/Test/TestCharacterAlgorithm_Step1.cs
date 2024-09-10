using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldHelper;
public class TestCharacterAlgorithm_Step1 : MonoBehaviour
{
    public HeroCharacter character;

    public bool isDesert_End = false;
    public bool isSnow_End = false;

    private void Start()
    {
        FieldManager.instance.AllFieldSpawn();
        GoField(FieldMap.Field.DESERT);   
    }

    private void Update()
    {
        if(character.isFieldEnter)
        {
            if(character.myField != FieldMap.Field.VILLAGE)
            {
                StartCoroutine(WaitForField(10f, FieldMap.Field.VILLAGE));
            }
            else if(!isDesert_End && character.myField == FieldMap.Field.VILLAGE)
            {
                StartCoroutine(WaitForField(10f, FieldMap.Field.DESERT));
            }
            else if(isDesert_End && !isSnow_End && character.myField == FieldMap.Field.VILLAGE)
            {
                StartCoroutine(WaitForField(10f, FieldMap.Field.SNOW));
            }
        }
    }
    protected void GoField(FieldMap.Field field)
    {
        character.targetField = FieldManager.instance.fieldList[(int)field];
    }

    protected IEnumerator WaitForField(float waitTime, FieldMap.Field nextField)
    {
        character.isFieldEnter = false;

        yield return new WaitForSeconds(waitTime);

        if(character.myField == FieldMap.Field.DESERT)
        {
            isDesert_End = true;
        }
        else if (character.myField == FieldMap.Field.SNOW)
        {
            isSnow_End = true;
        }

        if (isDesert_End && isSnow_End)
        {
            isDesert_End = false;
            isSnow_End = false;
        }
        GoField(nextField);
    }

}
