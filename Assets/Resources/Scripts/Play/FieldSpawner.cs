using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldHelper;
using StatusHelper;
public class FieldSpawner : MonoBehaviour, ICustomUpdateMono
{
    public Transform getTransform;
    public FieldMap.Field controlField;
    public bool isSpawn = false; //true일 경우 리스폰
    public int spawnUnitCount;
    public List<Transform> spawnTargets = new List<Transform>();

    public List<HunterCharacter> inCharacters = new List<HunterCharacter>();
    public LayerMask scanLayer;
    private GameObject unitPrefab;
    private bool onScanning = false;
    public Vector3 boxSize = new Vector3(1f, 1f, 1f);

    void Start()
    {
        unitPrefab = Resources.Load<GameObject>("Prefabs/Enemy/Enemy");
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
        if (isSpawn)
        {
            SpawnSetting();
            isSpawn = false;
        }

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
            HunterCharacter hunter = hitCollider.transform.parent.GetComponentInParent<HunterCharacter>();
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

    protected void SpawnSetting()
    {
        List<GameObject> prefabs = new List<GameObject>();
        List<Vector3> pos = new List<Vector3>();

        for (int i = 0; i < spawnUnitCount; i++)
        {
            prefabs.Add(unitPrefab);
        }
        pos = SpawnPositionSet(spawnUnitCount);
        UnitSpawn(prefabs, spawnUnitCount, pos);
    }
    protected void UnitSpawn(List<GameObject> prefab, int count, List<Vector3> spawnPos)
    {
        for(int i = 0; i < count; i++)
        {
            PoolManager.instance.Spawn(prefab[i], spawnPos[i], Vector3.one, Quaternion.identity, true, transform);    
        }
    }

    protected List<Vector3> SpawnPositionSet(int count)
    {
        List<Vector3> position = new List<Vector3>();
        HashSet<Vector3> existingPositions = new HashSet<Vector3>();

        // Ensure that the field has enough space to accommodate unique positions
        int maxAttempts = 10000;

        while (position.Count < count)
        {
            var (success, newPosition) = GenerateUniquePosition(existingPositions, maxAttempts);
            if (success)
            {
                position.Add(newPosition);
                existingPositions.Add(newPosition);
            }
            else
            {
                Debug.LogWarning("Unable to generate a unique position after 10,000 attempts.");
            }
        }

        return position;
    }

    private (bool, Vector3) GenerateUniquePosition(HashSet<Vector3> existingPositions, int maxAttempts)
    {
        int attempts = 0;
        Vector3 newPosition = Vector3.zero;


        do
        {


            attempts++;
        } 
        while 
        (
            existingPositions.Contains(newPosition) && attempts < maxAttempts
        );

        // Return a tuple containing a success flag and the new position
        bool success = attempts < maxAttempts;
        return (success, newPosition);
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
