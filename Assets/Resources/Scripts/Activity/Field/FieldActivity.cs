using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldHelper;
using StatusHelper;
using Util;

public class FieldActivity : MonoBehaviour, ICustomUpdateMono
{
    public Transform getTransform;
    public FieldSpawner mySpawner;

    [Header("FieldInfo")]
    [SerializeField] private FieldMap.Field fieldName;
    public FieldMap.Field FieldName
    {
        get { return fieldName; }
        private set { fieldName = value; }
    }
    public LayerMask scanLayer;
    public Vector3 boxSize = new Vector3(1f, 1f, 1f);
    [HideInInspector] public int maxBossPoint = 100;
    [Range(0, 100)] public int bossPoint = 0;

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
        StartCoroutine(ScanCharacter(0.1f));
        StartCoroutine(ScanEnemy(0.1f));
        BossSpawn();
    }

    protected IEnumerator ScanEnemy(float scanDelay)
    {
        if (isEnemyScanning)
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

            if (!isBossSpawned)
            {
                foreach (EnemyCharacter enemy in monsters)
                {
                    if (enemy.soonAttacker.Count >= enemy.soonAttackerLimit && enemy.soonAttackerLimit != -1)
                    {
                        continue;
                    }

                    //Ÿ������ ���� ���ʹ� �̸� �Ÿ������� �־���
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
            }
            else if(isBossSpawned)
            {
                foreach (EnemyCharacter enemy in bosses)
                {
                    if (enemy.soonAttacker.Count >= enemy.soonAttackerLimit && enemy.soonAttackerLimit != -1)
                    {
                        continue;
                    }

                    //Ÿ������ ���� ���ʹ� �̸� �Ÿ������� �־���
                    if (hero.targetUnit != null)
                    {
                        shortDistance = Vector3.Distance(hero.myObject.position, hero.targetUnit.position);
                        nearMonster = enemy;
                    }

                    float dis = Vector3.Distance(hero.myObject.position, enemy.myObject.position);
                    if (dis < shortDistance)
                    {
                        shortDistance = dis;
                        nearMonster = enemy;
                    }
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
                        if(!nearMonster.soonAttacker.Contains(hero))
                        {
                            nearMonster.soonAttacker.Add(hero);
                        }
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

        // ��ģ �ݶ��̴��� ���� ó��
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
        if (bossPoint < maxBossPoint || isBossSpawned)
        {
            return;      
        }

        Debug.Log("���� ��ȯ");
        bossPoint = 0;

        //ī�޶� �̵� ����
        CameraController_InGame camera = FieldManager.instance.cameraController;
        camera.subCameraUsable.AddCoroutine(camera.subCameraUsable.CameraBossTracking(camera.transform.position, getTransform.position, 40f, this));

        //���� ��ȯ ����
        mySpawner.BossSpawn();

        //��ĵ�� ĳ���͵��� Ÿ�� ����� and �罺ĵ
        foreach(HeroCharacter hero in inCharacters)
        {
            hero.targetUnit = null;
        }
        isEnemyScanning = false;
    }

    protected void CharacterFieldCalc(HeroCharacter character)
    {
        if(character.currentField != fieldName)
        {
            character.currentField = fieldName;
            character.isFieldEnter = true;

        }
    }
   

#if UNITY_EDITOR
    int segments = 100;
    bool drawWhenSelected = true;

    void OnDrawGizmosSelected()
    {
        if (drawWhenSelected)
        {
            for (float i = -0.02f; i <= 0.02f; i += 0.02f)
            {
                //Ž�� �þ�
                Gizmos.color = Color.red;
                Vector3 offset = new Vector3(i, i, i);
                Gizmos.DrawWireCube(getTransform.position + offset, boxSize);
            }
        }
    }

   
#endif
}
