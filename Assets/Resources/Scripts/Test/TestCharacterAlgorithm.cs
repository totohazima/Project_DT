using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldHelper;
public class TestCharacterAlgorithm : MonoBehaviour
{
    public HunterCharacter character;

    private void Start()
    {
        GoField(FieldMap.Field.DESERT);   
    }

    private void Update()
    {
        if(character.isFieldEnter)
        {
            if(character.myField != FieldMap.Field.VILLAGE)
            {
                StartCoroutine(WaitForField(30f, FieldMap.Field.VILLAGE));
            }
            else
            {
                StartCoroutine(WaitForField(30f, FieldMap.Field.DESERT));
            }
        }

    }
    protected void GoField(FieldMap.Field field)
    {
        character.targetLocation = FieldManager.instance.fieldList[(int)field].position;
    }

    protected IEnumerator WaitForField(float waitTime, FieldMap.Field nextField)
    {
        character.isFieldEnter = false;

        yield return new WaitForSeconds(waitTime);

        GoField(nextField);
    }

}
