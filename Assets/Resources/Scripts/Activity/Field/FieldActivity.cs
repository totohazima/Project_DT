using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldHelper;
using StatusHelper;
using Unity.VisualScripting;
public class FieldActivity : MonoBehaviour, ICustomUpdateMono
{
    public Transform getTransform;
    public FieldMap.Field controlField;
    public LayerMask scanLayer;
    public Vector3 boxSize = new Vector3(1f, 1f, 1f);
    public FieldSpawner mySpawner;
    [HideInInspector] public int maxBossPoint = 100;
    [Range(0, 100)]public int bossPoint = 0;
    
    [Header("Bool")]
    public bool isBossSpawned = false;
    private bool isHeroScanning = false;
    private bool isEnemyScanning = false;
    
    [Header("List")]
    public List<HeroCharacter> inCharacters = new List<HeroCharacter>();
    public List<EnemyCharacter> monsters = new List<EnemyCharacter>();
    public List<EnemyCharacter> bosses = new List<EnemyCharacter>();

    void Awake()
    {
        getTransform = GetComponent<Transform>();
        mySpawner = GetComponentInChildren<FieldSpawner>();
    }
    void OnEnable()
    {
       CustomUpdateManager.customUpdateMonos.Add(this);
    }
    void OnDisable()
    {
        CustomUpdateManager.customUpdateMonos.Remove(this);
    }

    public void CustomUpdate()
    {
        StartCoroutine(ScanCharacter(0.5f));
        StartCoroutine(ScanEnemy(0.1f));
        BossSpawn();
        AlwaysEliteTargetting();
    }

    protected IEnumerator ScanEnemy(float scanDelay)
    {
        if (isEnemyScanning || isBossSpawned)
        {
            yield break;
        }

        isEnemyScanning = true;

        foreach(HeroCharacter hero in inCharacters)
        {
            EnemyCharacter nearMonster = null;
            float shortDistance = Mathf.Infinity;

            if(hero.isStopScanning)
            {
                hero.soonTargetter = null;
                hero.targetUnit = null;
                continue;
            }


            foreach (EnemyCharacter enemy in monsters)
            {
                if (enemy.soonAttacker.Count >= enemy.soonAttackerLimit)
                {
                    continue;
                }

                //타겟으로 잡힌 몬스터는 변수에 거리를 넣어줌
                if (hero.targetUnit != null)
                {
                    shortDistance = Vector3.Distance(hero.myObject.position, hero.targetUnit.position);        
                }

                float dis = Vector3.Distance(hero.myObject.position, enemy.myObject.position);
                if (dis < shortDistance)
                {
                    shortDistance = dis;
                    nearMonster = enemy;
                }
            }

            if (nearMonster != null)
            {
                if (hero.soonTargetter != null)
                {
                    if (hero.soonTargetter != nearMonster)
                    {   
                        hero.soonTargetter.soonAttacker.Remove(hero);
                        hero.soonTargetter = nearMonster;
                        hero.soonTargetter.soonAttacker.Add(hero);
                        hero.targetUnit = hero.soonTargetter.myObject;
                    }
                    else
                    {
                        hero.targetUnit = hero.soonTargetter.myObject;
                    }
                }
                else
                {
                    hero.soonTargetter = nearMonster;
                    hero.soonTargetter.soonAttacker.Add(hero);
                    hero.targetUnit = hero.soonTargetter.myObject;
                }
            }
        }

        yield return new WaitForSeconds(scanDelay);
        isEnemyScanning = false;
    }

    protected IEnumerator ScanCharacter(float scanDelay)
    {
        if(isHeroScanning)
        {
            yield break;
        }

        isHeroScanning = true;

        inCharacters.Clear();

        Collider[] hitColliders = Physics.OverlapBox(getTransform.position, boxSize / 2, Quaternion.identity, scanLayer);

        // 겹친 콜라이더에 대해 처리
        foreach (Collider hitCollider in hitColliders)
        {
            foreach(HeroCharacter hero in FieldManager.instance.heroList)
            {
                if(hitCollider == hero.myCollider)
                {
                    inCharacters.Add(hero);
                    CharacterFieldCalc(hero);
                }
            }
        }

        yield return new WaitForSeconds(scanDelay);
        isHeroScanning = false;
    }

    protected void BossSpawn()
    {
        if (bossPoint < maxBossPoint || FieldManager.instance.isAlreadyBossSpawn || isBossSpawned)
        {
            return;      
        }

        Debug.Log("보스 소환");
        bossPoint = 0;

        //카메라 이동 연출
        CameraUsable camera = FieldManager.instance.cameraUsable;
        camera.subCameraUsable.AddCoroutine(camera.subCameraUsable.CameraBossTracking(camera.transform.position, getTransform.position, 20f, this));

        //보스 소환 연출
        StartCoroutine(mySpawner.BossSpawn());

        //스캔된 캐릭터들의 타겟을 보스로 고정시켜야 함
        HeroEliteCombatCalc(true);
    }

    protected void CharacterFieldCalc(HeroCharacter character)
    {
        if(character.myField != controlField)
        {
            character.myField = controlField;
            character.isFieldEnter = true;

        }
    }

    public void HeroEliteCombatCalc(bool isCombat)
    {
        foreach (HeroCharacter character in inCharacters)
        {
            character.isEliteCombat = isCombat;
        }
    }

    //보스가 스폰된 상태에서 필드 내 유닛들에게 보스 타겟팅
    public void AlwaysEliteTargetting()
    {
        if (isBossSpawned)
        {
            foreach (HeroCharacter character in inCharacters)
            {
                if (character.targetUnit == null)
                {
                    character.targetUnit = bosses[0].myObject;
                }
            }
        }
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
            Gizmos.DrawWireCube(getTransform.position, boxSize);
        }
    }

   
#endif
}
