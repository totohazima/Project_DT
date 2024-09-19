using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldHelper;
using Unity.VisualScripting;
public class TestCharacterAlgorithm_Step2 : MonoBehaviour
{
    public int cycleCount = 0;
    public HeroCharacter character;
    public float waitSecond = 30f;
    [SerializeField] List<EnemyCharacter> enemies = new List<EnemyCharacter>();

    private void Start()
    {
        FieldManager.instance.AllFieldSpawn();

        StartCoroutine(Step1_GoDesert());
    }

    protected IEnumerator Step1_GoDesert()
    {
        character.targetField = FieldManager.instance.fieldList[(int)FieldMap.Field.DESERT];

        while(true)
        {
            if(character.myField == FieldMap.Field.DESERT)
            {
                StartCoroutine(Step2_HuntDesert());
                yield break;
            }

            yield return null;
        }
        
    }

    protected IEnumerator Step2_HuntDesert()
    {
        enemies = FieldManager.instance.fields[(int)FieldMap.Field.DESERT].monsters;

        while (true)
        {
            if(enemies.Count == 0)
            {
                yield return new WaitForSeconds(waitSecond);
                StartCoroutine(Step3_GoVillage());
                yield break;
            }

            //if(character.targetUnit == null)
            //{
            //    character.targetUnit = TargetReSetting();
            //}

            yield return null;
        }
    }

    protected IEnumerator Step3_GoVillage()
    {
        character.targetField = FieldManager.instance.fieldList[(int)FieldMap.Field.VILLAGE];

        while (true)
        {
            if (character.myField == FieldMap.Field.VILLAGE)
            {
                yield return new WaitForSeconds(waitSecond);
                StartCoroutine(Step4_GoSnow());
                yield break;
            }

            yield return null;
        }
    }

    protected IEnumerator Step4_GoSnow()
    {
        character.targetField = FieldManager.instance.fieldList[(int)FieldMap.Field.SNOW];

        while (true)
        {
            if (character.myField == FieldMap.Field.SNOW)
            {
                StartCoroutine(Step5_HuntSnow());
                yield break;
            }

            yield return null;
        }
    }

    protected IEnumerator Step5_HuntSnow()
    {
        enemies = FieldManager.instance.fields[(int)FieldMap.Field.SNOW].monsters;

        while (true)
        {
            if (enemies.Count == 0)
            {
                yield return new WaitForSeconds(waitSecond);
                StartCoroutine(Step6_GoVillage());
                yield break;
            }

            //if (character.targetUnit == null)
            //{
            //    character.targetUnit = TargetReSetting();
            //}

            yield return null;
        }
    }

    protected IEnumerator Step6_GoVillage()
    {
        character.targetField = FieldManager.instance.fieldList[(int)FieldMap.Field.VILLAGE];

        while (true)
        {
            if (character.myField == FieldMap.Field.VILLAGE)
            {
                yield return new WaitForSeconds(waitSecond);
                cycleCount++;
                Debug.Log("알고리즘 실행 횟수: " + cycleCount);
                FieldManager.instance.AllFieldSpawn();
                StartCoroutine(Step1_GoDesert());
                yield break;
            }

            yield return null;
        }
    }


    private Transform TargetReSetting()
    {
        Transform enemy = null;
        enemy = enemies[0].myObject;

        return enemy;
    }

}
