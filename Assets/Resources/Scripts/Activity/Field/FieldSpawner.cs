using GameSystem;
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
    [SerializeField] private float reSpawnReadyTime;
    private float reSpawnReadyTimer;

    [SerializeField] private List<EnemyCharacter> monster = new List<EnemyCharacter>();
    private EnemyCharacter boss;

    void Awake()
    {
        GameObject unitPrefab = Resources.Load<GameObject>("Prefabs/Enemys/mn_000");
        EnemyCharacter mn = unitPrefab.GetComponent<EnemyCharacter>();
        monster.Add(mn);

        unitPrefab = Resources.Load<GameObject>("Prefabs/Enemys/mn_002");
        mn = unitPrefab.GetComponent<EnemyCharacter>();
        monster.Add(mn);


        GameObject boosPrefab = Resources.Load<GameObject>("Prefabs/Enemys/mn_000_Boss");
        boss = boosPrefab.GetComponent<EnemyCharacter>();
    }
    private void OnEnable()
    {
        CustomUpdateManager.customUpdateMonos.Add(this);   
    }
    public void CustomUpdate()
    {
        if(isSpawn)
        {
            SpawnSetting(spawnUnitCount);
            isSpawn = false;
        }
        else if(fieldActivity.monsters.Count < spawnUnitCount && reSpawnReadyTimer >= reSpawnReadyTime)
        {
            SpawnSetting(1);
            reSpawnReadyTimer = 0f;
        }

        reSpawnReadyTimer += Time.deltaTime;
    }
    private void OnDisable()
    {
        CustomUpdateManager.customUpdateMonos.Remove(this);
    }
    protected void SpawnSetting(int count)
    {
        List<EnemyCharacter> prefabs = new List<EnemyCharacter>();
        List<Vector3> pos = new List<Vector3>();

        for(int i = 0; i < count; i++)
        {
            int num = Random.Range(0, monster.Count);
            prefabs.Add(monster[num]);
        }

        pos = SpawnPointSet(count);
        UnitSpawn(prefabs, count, pos);

    }

    //protected void AllSpawnSetting()
    //{
    //    List<EnemyCharacter> prefabs = new List<EnemyCharacter>();
    //    List<Vector3> pos = new List<Vector3>();

    //    for (int i = 0; i < spawnUnitCount; i++)
    //    {
    //        int num = Random.Range(0, monster.Count);
    //        prefabs.Add(monster[num]);
    //    }
    //    pos = SpawnPointSet(spawnUnitCount);
    //    UnitSpawn(prefabs, spawnUnitCount, pos);

    //}


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

        for (int i = 0; i < count; i++)
        {
            Vector3 boxSize = fieldActivity.boxSize;
            // ������ �ڽ� ������ ������ ��ġ ����
            Vector3 randomPositionWithinBox = new Vector3(
                Random.Range(-boxSize.x / 2, boxSize.x / 2),
                Random.Range(-boxSize.y / 2, boxSize.y / 2),
                0
            );

            Vector3 targetPos = fieldActivity.getTransform.position + randomPositionWithinBox;
            pos.Add(targetPos);
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
