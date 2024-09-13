using GameEvent;
using GameSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HeroCharacter : Character, IPointerClickHandler
{
    [Header("Field and Movement Info")]
    public bool isFieldEnter = false;
    public bool onRandomMove = false;

    private float randomMoveRadius = 2f;
    private float randomMoveTime;
    private float randomMoveTime_Max = 10f;
    private float randomMoveTime_Min = 3f;

    [Header("Scanning Info")]
    private float scanDelay = 0.1f;
    private bool isScanning = false;

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
    }

    public override void Update()
    {
        if(isDisable || isDead)
        {
            return;
        }

        StartCoroutine(ObjectScan(scanDelay));
        //StartCoroutine(RandomMoveLocation());
        StatCalculate();
        StatusUpdate();
        AnimationUpdate();
        
        if(attackEvent != null)
        {
            eventListener = new UnityEvent();
            attackEvent.RegisterListener(gameObject, eventListener);
            eventCallAnimation.callPrefab = attackPrefab;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!onClickProcess)
        {
            StartCoroutine(ProcessClick(eventData.position));
        }
    }

    private IEnumerator RandomMoveLocation()
    {
        if (onRandomMove)
        {
            yield break;
        }
        onRandomMove = true;
        randomMoveTime = Random.Range(randomMoveTime_Min, randomMoveTime_Max);

        yield return new WaitForSeconds(randomMoveTime);

        Vector3 boxSize = FieldManager.instance.fields[(int)myField].boxSize;
        // 오버랩 박스 내에서 무작위 위치 생성
        Vector3 randomPositionWithinBox = new Vector3(
            Random.Range(-boxSize.x / 2, boxSize.x / 2),
            Random.Range(-boxSize.y / 2, boxSize.y / 2),
            Random.Range(-boxSize.z / 2, boxSize.z / 2)
        );

        // 현재 위치에 대해 상대적인 위치를 적용하여 이동
        FieldActivity controlField = FieldManager.instance.fields[(int)myField];
        targetLocation = controlField.getTransform.position + randomPositionWithinBox;

        onRandomMove = false;  // 이동 종료
    }
    public override IEnumerator ObjectScan(float scanDelay)
    {
        if (!isScanning)
        {
            isScanning = true;

            Collider[] detectedColls = Physics.OverlapSphere(myObject.position, (float)playStatus.viewRange, 1 << 7);
            float shortestDistance = Mathf.Infinity;
            Transform nearestTarget = null;

            foreach (Collider col in detectedColls)
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
                targetUnit = nearestTarget;
                AttackRangeScan();
            }
            else
            {
                targetUnit = null;
                isReadyToAttack = false;
            }


            yield return new WaitForSeconds(scanDelay);
            isScanning = false;
        }
    }
    private void AttackRangeScan()
    {
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
        }
        else if (targetField != null)
        {
            SetTargetPosition(targetField.position);
        }
        else if (targetLocation != Vector3.zero)
        {
            SetTargetPosition(targetLocation);
        }
        else
        {
            isMove = false;
        }
    }

    private void UpdateAttackStatus()
    {
        if (isReadyToAttack) 
            isMove = false;
    }

    private void UpdateLerpSpeed()
    {
        aiLerp.canMove = isMove;
        aiLerp.speed = stateController.moveSpeed;

        if (aiLerp.reachedDestination || aiLerp.reachedEndOfPath)
        {
            isMove = false;
        }
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

        if(isReadyToAttack)
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

        yield return new WaitForSeconds(1f);

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
                FieldManager.instance.cameraDrag.trackingTarget = this;
            }
        }

        yield return 0;
    }

}
