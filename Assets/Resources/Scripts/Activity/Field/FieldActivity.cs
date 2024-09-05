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
    public List<HunterCharacter> inCharacters = new List<HunterCharacter>();
    public LayerMask scanLayer;
    private bool onScanning = false;
    public Vector3 boxSize = new Vector3(1f, 1f, 1f);
    [Header("Monsters")]
    public List<EnemyCharacter> monsters = new List<EnemyCharacter>();
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
    }

    protected IEnumerator ScanCharacter()
    {
        onScanning = true;

        inCharacters.Clear();

        Collider[] hitColliders = Physics.OverlapBox(getTransform.position, boxSize / 2, Quaternion.identity, scanLayer);

        // 겹친 콜라이더에 대해 처리
        foreach (Collider hitCollider in hitColliders)
        {
            HunterCharacter hunter = hitCollider.transform.GetComponentInParent<HunterCharacter>();
            if (hunter != null)
            {
                inCharacters.Add(hunter);
                CharacterFieldSetting(hunter);
            }
        }

        yield return new WaitForSeconds(0.5f);
        onScanning = false;
    }
    protected void CharacterFieldSetting(HunterCharacter character)
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
