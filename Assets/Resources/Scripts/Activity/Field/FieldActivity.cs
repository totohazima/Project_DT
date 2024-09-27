using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldHelper;
using StatusHelper;
public class FieldActivity : MonoBehaviour, ICustomUpdateMono
{
    public Transform getTransform;
    public FieldMap.Field controlField;
    public List<HeroCharacter> inCharacters = new List<HeroCharacter>();
    public LayerMask scanLayer;
    private bool onScanning = false;
    public Vector3 boxSize = new Vector3(1f, 1f, 1f);
    [Header("Monsters")]
    public List<EnemyCharacter> monsters = new List<EnemyCharacter>();
    [Header("Boss")]
    [HideInInspector] public int maxBossPoint = 100;
    public int bossPoint = 0;
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
        if(!onScanning)
        {
            StartCoroutine(ScanCharacter());
        }

        if(bossPoint >= maxBossPoint)
        {
            //bossPoint = maxBossPoint;
            BossSpawn();
        }
    }

    protected void BossSpawn()
    {
        Debug.Log("보스 소환");
        bossPoint = 0;
        
        //보스 소환 애니메이션
        //카메라 이동 애니메이션
        //스캔된 캐릭터들의 타겟을 보스로 고정시켜야 함
    }

    protected IEnumerator ScanCharacter()
    {
        onScanning = true;

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
                    CharacterFieldSetting(hero);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        onScanning = false;
    }
    protected void CharacterFieldSetting(HeroCharacter character)
    {
        if(character.myField != controlField)
        {
            character.myField = controlField;
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
            //탐지 시야
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(getTransform.position, boxSize);
        }
    }

   
#endif
}
