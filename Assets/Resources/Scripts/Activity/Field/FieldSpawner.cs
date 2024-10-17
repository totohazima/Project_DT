using GameSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;

public class FieldSpawner : MonoBehaviour, ICustomUpdateMono
{
    public FieldActivity fieldActivity;
    public bool isSpawn = false; //true�� ��� ������
    public int spawnUnitCount; //���� �� ����
    public Transform spawnPointGroup;
    public List<Transform> spawnPoints = new List<Transform>();

    private GameObject unitPrefab;
    private EnemyCharacter monster;

    private GameObject boosPrefab;
    private EnemyCharacter boss;

    void Awake()
    {
        for(int i = 0; i < spawnPointGroup.childCount; i++)
        {
            spawnPoints.Add(spawnPointGroup.GetChild(i).transform);
        }

        unitPrefab = Resources.Load<GameObject>("Prefabs/Enemys/mn_000");
        monster = unitPrefab.GetComponent<EnemyCharacter>();

        boosPrefab = Resources.Load<GameObject>("Prefabs/Enemys/mn_000_Boss");
        boss = boosPrefab.GetComponent<EnemyCharacter>();
    }
    private void OnEnable()
    {
        CustomUpdateManager.customUpdateMonos.Add(this);   
    }
    public void CustomUpdate()
    {
        if(isSpawn || fieldActivity.monsters.Count == 0)
        {
            SpawnSetting();
            isSpawn = false;
        }
    }
    private void OnDisable()
    {
        CustomUpdateManager.customUpdateMonos.Remove(this);
    }

    protected void SpawnSetting()
    {
        List<EnemyCharacter> prefabs = new List<EnemyCharacter>();
        List<Vector3> pos = new List<Vector3>();

        for (int i = 0; i < spawnUnitCount; i++)
        {
            prefabs.Add(monster);
        }
        pos = SpawnPointSet(spawnUnitCount);
        UnitSpawn(prefabs, spawnUnitCount, pos);

    }
    protected void UnitSpawn(List<EnemyCharacter> prefab, int count, List<Vector3> spawnPos)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject monster = PoolManager.instance.Spawn(prefab[i].gameObject, spawnPos[i], Vector3.one, Quaternion.identity, true, FieldManager.instance.spawnPool);

            EnemyCharacter monsterCharacter = monster.GetComponent<EnemyCharacter>();
            monsterCharacter.currentField = fieldActivity.fieldName;
            monsterCharacter.targetField = fieldActivity.fieldName;
            //monsterCharacter.soonAttackerLimit = 2;
            fieldActivity.monsters.Add(monsterCharacter);
        }
    }

    //������ ���� ��Ű�� ���� ���� ����
    public void BossSpawn()
    {
        fieldActivity.isBossSpawned = true;

        GameObject monster = PoolManager.instance.Spawn(boss.gameObject, fieldActivity.getTransform.position, Vector3.one, Quaternion.identity, true, FieldManager.instance.spawnPool);

        EnemyCharacter monsterCharacter = monster.GetComponent<EnemyCharacter>();
        monsterCharacter.currentField = fieldActivity.fieldName;
        monsterCharacter.targetField = fieldActivity.fieldName;
        fieldActivity.bosses.Add(monsterCharacter);
        monsterCharacter.enabled = false;

        StartCoroutine(BossSpawnAnimation(monsterCharacter, 1.5f));
    }

    protected List<Vector3> SpawnPointSet(int count)
    {
        List<Vector3> pos = new List<Vector3>();

        float[] chanceList = new float[spawnPoints.Count];

        for (int i = 0; i < chanceList.Length; i++)
        {
            chanceList[i] = 1f / chanceList.Length; // �� ���� ����Ʈ�� Ȯ���� �����ϰ� ����
        }

        for (int i = 0; i < count; i++)
        {
            int index = GameManager.instance.Judgment(chanceList);
            Vector3 randPos = spawnPoints[index].position;
            pos.Add(randPos);
        }
        return pos;
    }

    protected IEnumerator BossSpawnAnimation(EnemyCharacter boss, float fadeDuration)
    {
        foreach (SpriteRenderer sprite in boss.spriteGroup.spriteRenderers)
        {
            StartCoroutine(FadeSprite(sprite, fadeDuration, 0f, 1f)); // ���̵� ��: ���� 0���� 1��
        }

        yield return new WaitForSeconds(fadeDuration); // ��� ���̵尡 ���� ������ ���

        boss.enabled = true;
    }

    private IEnumerator FadeSprite(SpriteRenderer spriteRenderer, float duration, float startAlpha, float endAlpha)
    {
        // �ʱ� ������ ������
        Color color = spriteRenderer.color;
        float elapsed = 0f;

        // duration ���� ���̵� ȿ��
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // ���İ��� ���������� ��ȭ
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            spriteRenderer.color = color;

            yield return null; // ���� �����ӱ��� ���
        }

        // ���� ���İ� ���� (������ �����ӿ��� ��Ȯ�� ���߱� ����)
        color.a = endAlpha;
        spriteRenderer.color = color;
    }


}
