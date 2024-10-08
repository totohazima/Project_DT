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
    public bool isSpawn = false; //true일 경우 리스폰
    public int spawnUnitCount; //유닛 수 제한
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
            monsterCharacter.myField = fieldActivity.controlField;
            monsterCharacter.targetField = fieldActivity.controlField;
            //monsterCharacter.soonAttackerLimit = 2;
            fieldActivity.monsters.Add(monsterCharacter);
        }
    }

    //보스를 스폰 시키고 보스 등장 연출
    public IEnumerator BossSpawn()
    {
        FieldManager.instance.isAlreadyBossSpawn = true;

        GameObject monster = PoolManager.instance.Spawn(boss.gameObject, fieldActivity.getTransform.position, Vector3.one, Quaternion.identity, true, FieldManager.instance.spawnPool);

        EnemyCharacter monsterCharacter = monster.GetComponent<EnemyCharacter>();
        monsterCharacter.myField = fieldActivity.controlField;
        monsterCharacter.targetField = fieldActivity.controlField;
        //monsterCharacter.soonAttackerLimit = -1;
        fieldActivity.bosses.Add(monsterCharacter);
        monsterCharacter.enabled = false;

        StartCoroutine(BossSpawnAnimation(monsterCharacter, 3f));
        yield return new WaitForSeconds(3.5f);

        monsterCharacter.enabled = true;
        fieldActivity.isBossSpawned = true;
        FieldManager.instance.isAlreadyBossSpawn = false;
    }

    protected List<Vector3> SpawnPointSet(int count)
    {
        List<Vector3> pos = new List<Vector3>();

        float[] chanceList = new float[spawnPoints.Count];

        for (int i = 0; i < chanceList.Length; i++)
        {
            chanceList[i] = 1f / chanceList.Length; // 각 스폰 포인트의 확률을 동일하게 설정
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
            StartCoroutine(FadeSprite(sprite, fadeDuration, 0f, 1f)); // 페이드 인: 알파 0에서 1로
        }

        yield return new WaitForSeconds(fadeDuration); // 모든 페이드가 끝날 때까지 대기
    }

    private IEnumerator FadeSprite(SpriteRenderer spriteRenderer, float duration, float startAlpha, float endAlpha)
    {
        // 초기 색상을 가져옴
        Color color = spriteRenderer.color;
        float elapsed = 0f;

        // duration 동안 페이드 효과
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 알파값을 점진적으로 변화
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            spriteRenderer.color = color;

            yield return null; // 다음 프레임까지 대기
        }

        // 최종 알파값 적용 (마지막 프레임에서 정확히 맞추기 위해)
        color.a = endAlpha;
        spriteRenderer.color = color;
    }


}
