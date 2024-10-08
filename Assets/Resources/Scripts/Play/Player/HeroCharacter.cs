using FieldHelper;
using GameEvent;
using GameSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HeroCharacter : Character, IPointerClickHandler
{
    [Header("Field and Movement Info")]
    public bool isFieldEnter = false;
    public bool onRandomMove = false;
    public bool isWaitingBuilding = false;
    public bool isEliteCombat = false;
    private float randomMoveRadius = 2f;
    private float randomMoveTime;
    private float randomMoveTime_Max = 10f;
    private float randomMoveTime_Min = 3f;
    [Header("Scanning Info")]
    private float scanDelay = 0.1f;
    private bool isScanning = false;
    public EnemyCharacter soonTargetter = null;
    [Header("Click Process Info")]
    protected bool onClickProcess;

    [Header("Game Event Info")]
    public EventCallAnimation eventCallAnimation = null;
    public GameObject attackPrefab;
    public GameEventFilter attackEvent = null;
    UnityEvent eventListener = null;

    public override void ReCycle()
    {
        base.ReCycle();
        onRandomMove = false;
        isFieldEnter = false;
        isScanning = false;
        isEliteCombat = false;
        soonTargetter = null;
        characterCostume.CostumeEquip_Process();

        if (attackEvent != null)
        {
            eventListener = new UnityEvent();
            attackEvent.RegisterListener(gameObject, eventListener);
            eventCallAnimation.callPrefab = attackPrefab;
        }
    }

    public override void CustomUpdate()
    {
        if(isDead)
        {
            return;
        }

        //StartCoroutine(RandomMoveLocation(myField));
        //StartCoroutine(ObjectScan(scanDelay));
        AttackRangeScan();
        StatCalculate();
        StatusUpdate();
        AnimationUpdate();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!onClickProcess)
        {
            StartCoroutine(ProcessClick(eventData.position));
        }
    }

    private IEnumerator RandomMoveLocation(FieldMap.Field field)
    {
        if (onRandomMove)
        {
            yield break;
        }

        onRandomMove = true;
        randomMoveTime = Random.Range(randomMoveTime_Min, randomMoveTime_Max);

        yield return new WaitForSeconds(randomMoveTime);

        Vector3 boxSize = FieldManager.instance.fields[(int)field].boxSize;
        // 오버랩 박스 내에서 무작위 위치 생성
        Vector3 randomPositionWithinBox = new Vector3(
            Random.Range(-boxSize.x / 2, boxSize.x / 2),
            Random.Range(-boxSize.y / 2, boxSize.y / 2),
            Random.Range(-boxSize.z / 2, boxSize.z / 2)
        );

        // 현재 위치에 대해 상대적인 위치를 적용하여 이동
        FieldActivity controlField = FieldManager.instance.fields[(int)field];
        targetLocation = controlField.getTransform.position + randomPositionWithinBox;

        onRandomMove = false;  // 이동 종료
    }

    public override IEnumerator ObjectScan(float scanDelay)
    {
        if (isStopScanning || isEliteCombat || isScanning)
        {
            yield break;
        }

        isScanning = true;

        ///주변 적 탐지가 아닌 필드 내 몬스터의 정보를 가져오는 방식
        FieldActivity field = FieldManager.instance.fields[(int)myField];
        List<Collider> detectedColls = new List<Collider>();
        List<Collider> detectedList = new List<Collider>(); //타겟으로 잡을 수 있는 상태의 몬스터들을 담음
        float shortestDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (EnemyCharacter enemy in field.monsters)
        {
            detectedColls.Add(enemy.myCollider);
        }

        foreach (Collider col in detectedColls)
        {
            if (col == null || col == myCollider)
            {
                continue;
            }

            for (int i = 0; i < field.monsters.Count; i++)
            {
                if (col == field.monsters[i].myCollider)
                {
                    if (field.monsters[i].isUntargetted == false)
                    {
                        detectedList.Add(field.monsters[i].myCollider);
                    }
                }
            }
        }

        foreach (Collider col in detectedList)
        {
            if (col == null || col == myCollider)
            {
                continue;
            }

            Transform target = col.transform;
            float dis = Vector3.Distance(myObject.position, target.position);

            if (dis < shortestDistance)
            {
                shortestDistance = dis;
                nearestTarget = target;
            }
        }

        if (nearestTarget != null)
        {
            if (nearestTarget != targetUnit)
            {
                bool isTargeting = false;

                EnemyCharacter target = null;

                foreach (EnemyCharacter enemy in field.monsters)
                {
                    if (enemy.myCollider.transform == nearestTarget)
                    {
                        target = enemy;
                    }
                }

                //기존 타겟이 아예 없는 경우 -> 새로 잡은 타겟이 기존 타겟이 되며 어태커 리스트에 자신을 추가
                //새로 잡은 타겟이 기존에 잡은 타겟과 같은 경우 -> 아무것도 할 필요 없음
                //새로 잡은 타겟이 기존 타겟과 다른 경우->기존 타겟 어태커 리스트에서 자신을 지움 이후 새 타겟의 어태커 리스트에 자신을 추가
                if (soonTargetter == null && target != null)
                {
                    soonTargetter = target;

                    if (soonTargetter.soonAttacker.Count < soonTargetter.soonAttackerLimit)
                    {
                        soonTargetter.soonAttacker.Add(this);
                        isTargeting = true;
                    }
                }
                else if (soonTargetter == target && target != null)
                {
                    soonTargetter = target;
                    if (soonTargetter.soonAttacker.Count < soonTargetter.soonAttackerLimit)
                    {
                        isTargeting = true;
                    }
                }
                else if (soonTargetter != target && target != null)
                {
                    soonTargetter.soonAttacker.Remove(this);

                    soonTargetter = target;

                    if (soonTargetter.soonAttacker.Count < soonTargetter.soonAttackerLimit)
                    {
                        soonTargetter.soonAttacker.Add(this);
                        isTargeting = true;
                    }
                }

                if (isTargeting)
                    targetUnit = soonTargetter.myObject;

            }
        }
        else
        {
            targetUnit = null;
        }


        yield return new WaitForSeconds(scanDelay);
        isScanning = false;
    }

    private void AttackRangeScan()
    {
        if(targetUnit == null)
        {
            isReadyToAttack = false;
            return;
        }

        float distance = Vector3.Distance(myObject.position, targetUnit.position);

        if(distance <= playStatus.attackRange)
        {
            isReadyToAttack = true;
        }
        else
        {
            isReadyToAttack = false;
        }
    }

    public override void StatusUpdate()
    {
        UpdateMovementStatus();
        UpdateAttackStatus();
        UpdateLerpSpeed();
    }

    public override void AnimationUpdate()
    {
        UpdateMovementAnimation();
        UpdateAttackAnimation();
    }


    private void UpdateMovementStatus()
    {
        if (targetUnit != null)
        {
            SetTargetPosition(targetUnit.position);
            if(isFieldEnter) //필드 진입 시 더 이상 필드 중앙으로 이동할 필요 X
            {
                isFieldEnter = false;
                targetField = myField;
            }

            if(!targetUnit.gameObject.activeSelf)
            {
                targetUnit = null;
            }
        }                                                      
        else if (targetField != myField)
        { 
            if(!isMove) //UpdateLerpSpeed가 이 메서드보다 늦게 실행되어 onTargetFieldPos가 true가 되어 움직이지 못하는 현상 방지
            {
                onTargetFieldPos = false;
            }

            if (!onTargetFieldPos)
            {
                onTargetFieldPos = true;

                Vector3 fieldPos = Vector3.zero;
                FieldActivity controlField = FieldManager.instance.fields[(int)targetField];
                Vector3 boxSize = controlField.boxSize;

                Vector3 randomPositionWithinBox = new Vector3(
                    Random.Range(-boxSize.x / 2, boxSize.x / 2),
                    Random.Range(-boxSize.y / 2, boxSize.y / 2),
                    Random.Range(-boxSize.z / 2, boxSize.z / 2)
                );

               
                fieldPos = controlField.getTransform.position + randomPositionWithinBox;

                SetTargetPosition(fieldPos);
            }
        }
        else if(targetBuilding != Building.BuildingType.NONE)
        {   
            if(!isMove)
            {
                onTargetBuildingPos = false;
            }

            if (!onTargetBuildingPos)
            {
                onTargetBuildingPos = true;

                Building building = BuildingManager.Instance.buildings[(int)targetBuilding - 1];

                Vector3 buildingPos = GetRandomPositionOutsideBox2();
                buildingPos = building.myObject.position + buildingPos;
                SetTargetPosition(buildingPos);
            }
        }
        else if (targetLocation != Vector3.zero)
        {
            SetTargetPosition(targetLocation);
        }
        else
        {
            //isMove = false;
        }

        if(targetField == myField)
        {
            onTargetFieldPos = false;
        }
        
    }
    private Vector2 GetRandomPositionOutsideBox2()
    {
        Building building = BuildingManager.Instance.buildings[(int)targetBuilding - 1];

        // 박스1 경계 계산
        float leftBox1 = -building.interactionRange.x / 2;
        float rightBox1 = building.interactionRange.x / 2;
        float bottomBox1 = -building.interactionRange.y / 2;
        float topBox1 = building.interactionRange.y / 2;

        // 콜리더 경계 계산 (박스1 중심 기준)
        float leftBox2 = -building.myCollider2D.bounds.size.x / 2;
        float rightBox2 = building.myCollider2D.bounds.size.x / 2;
        float bottomBox2 = -building.myCollider2D.bounds.size.y / 2;
        float topBox2 = building.myCollider2D.bounds.size.y / 2;

        while (true)
        {
            // 박스1 범위 내에서 랜덤 좌표 선택
            float randomX = Random.Range(leftBox1, rightBox1);
            float randomY = Random.Range(bottomBox1, topBox1);

            // 선택된 좌표가 박스2 범위에 닿지 않는다면 반환
            if (!(randomX > leftBox2 && randomX < rightBox2 && randomY > bottomBox2 && randomY < topBox2))
            {
                return new Vector2(randomX, randomY);
            }
        }
    }
    private void UpdateAttackStatus()
    {
        if (isReadyToAttack)
        {
            isMove = false;
        }

        if(myField == FieldMap.Field.VILLAGE)
        {
            isStopScanning = true;
        }
        else
        {
            isStopScanning = false; 
        }
    }

    private void UpdateLerpSpeed()
    {
        if (aiLerp.reachedDestination || aiLerp.reachedEndOfPath || isWaitingBuilding)
        {
            isMove = false;
        }

        aiLerp.canMove = isMove;
        aiLerp.speed = stateController.moveSpeed;

    }
    private void SetTargetPosition(Vector3 targetPosition)
    {
        isMove = true;
        aiLerp.destination = targetPosition;
    }

    private void UpdateMovementAnimation()
    {
        if (isMove)
        {
            anim.SetBool(AnimatorParams.MOVE, true);
        }
        else
        {
            anim.SetBool(AnimatorParams.MOVE, false);
        }
        SetDirection();
    }

    private void SetDirection()
    {
        if (isMove)
        {
            if (aiLerp.steeringTarget.x < myObject.position.x)
            {
                viewObject.rotation = Quaternion.Euler(0, 180, 0); // Left
            }
            else
            {
                viewObject.rotation = Quaternion.Euler(0, 0, 0); // Right
            }
        }

        if(isReadyToAttack && targetUnit != null)
        {
            if (targetUnit.position.x < myObject.position.x)
            {
                viewObject.rotation = Quaternion.Euler(0, 180, 0); // Left
            }
            else
            {
                viewObject.rotation = Quaternion.Euler(0, 0, 0); // Right
            }
        }
    }

    private void UpdateAttackAnimation()
    {
        anim.SetBool(AnimatorParams.DEVILATTACK_1, isReadyToAttack);
    }

    public override IEnumerator Death()
    {
        if(!anim.GetBool(AnimatorParams.DEATH))
        {
            anim.SetBool(AnimatorParams.DEATH, true);
        }

        myCollider.enabled = false;
        isReadyToMove = false;
        
        if(kda_Controller != null)
        {
            //kda_Controller.KDA_Calculator()
        }
        yield return new WaitForSeconds(0.1f);

        myCollider.enabled = true;
        Disappear();
    }

    /// <summary>
    /// 캐릭터 클릭 시 추적
    /// </summary>
    public IEnumerator ProcessClick(Vector3 clickPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(clickPosition);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == myCollider)
            {
                FieldManager.instance.cameraUsable.trackingTarget = this;
            }
        }

        yield return 0;
    }

    /// <summary>
    /// 건물 상호작용 애니메이션
    /// </summary>
    public void Builng_Use(Building building, float useDelay)
    {
        StartCoroutine(Building_Interaction(building, useDelay));    
    }
    protected IEnumerator Building_Interaction(Building building, float useDelay)
    {
        //Debug.Log(characterName + building.buildingName + " 상호작용 시작");
        ItemPopup(GameManager.instance.GetRandomEnumValue<GameMoney.GameMoneyType>(0), Random.Range(1, 4));
        building.RewardPopup();

        yield return new WaitForSeconds(useDelay);

        targetBuilding = Building.BuildingType.NONE;
        isWaitingBuilding = false;
        building.isInteraction = false;
        building.customerList.Remove(this);
        //Debug.Log(characterName + building.buildingName + " 상호작용 완료");
    }

    protected void ItemPopup(GameMoney.GameMoneyType type, int count)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/FieldObject/PopupItem");
        GameObject popup = PoolManager.instance.Spawn(prefab, popupPos.position, Vector3.one, Quaternion.identity, true, myObject.parent);

        PopupItem popupItem = popup.GetComponent<PopupItem>();
        popupItem.ItemSetting(type, count);
    }
}
