using GameEvent;
using GameSystem;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using static Pathfinding.Examples.Interactable.TeleportAgentOnLinkAction;

public class EnemyCharacter : Character
{
    [Header("RandomMove_Info")]
    private bool onRandomMove = false;
    //private float randomMoveRadius = 2f; //이 만큼의 거리내로 랜덤 이동
    private float randomMoveTime; //이 시간 동안 타겟이 잡히지 않으면 일정 거리 내 위치로 랜덤 이동
    private float randomMoveTime_Max = 5f;
    private float randomMoveTime_Min = 1f;
    [Header("ScanTime_Info")]
    private float scanDelay = 0.1f; //스캔이 재작동하는 시간
    private bool isScanning = false; //스캔 코루틴이 실행중인지 체크하는 변수
    [Header("ItemDrop_Info")]
    public int dropItemCount = 1; //아이템 총 드랍 개수
    [Header("GameEvent")]
    public EventCallAnimation eventCallAnimation = null;
    public GameObject attackPrefab;
    public GameEventFilter attackEvent = null;
    UnityEvent eventListener = null;

    public override void ReCycle()
    {
        base.ReCycle();
        onRandomMove = false;
        isScanning = false;
    }
    public override void CustomUpdate()
    {
        if(isDisable || isDead)
        {
            return;
        }

        StartCoroutine(RandomMoveLocation());
        StartCoroutine(ObjectScan(scanDelay));
        StatCalculate();
        StatusUpdate();
        AnimationUpdate();

        if (attackEvent != null)
        {
            eventListener = new UnityEvent();
            attackEvent.RegisterListener(gameObject, eventListener);
            eventCallAnimation.callPrefab = attackPrefab;
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

            yield return new WaitForSeconds(scanDelay);

            Collider[] detectedColls = Physics.OverlapSphere(myObject.position, (float)playStatus.viewRange, 1 << 8);
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
            }
            else
            {
                targetUnit = null;
                isReadyToAttack = false;
            }

            AttackRangeScan();
            isScanning = false;
        }
    }

    private void AttackRangeScan()
    {
        if(targetUnit == null)
        {
            return;
        }

        float distance = Vector3.Distance(myObject.position, targetUnit.position);

        if (distance <= playStatus.attackRange)
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
        if (isReadyToAttack) isMove = false;
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
        else if (isReadyToAttack)
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
        anim.SetBool(AnimatorParams.ATTACK_1, isReadyToAttack);
    }


    public override IEnumerator Death()
    {
        if (!anim.GetBool(AnimatorParams.DEATH))
        {
            anim.SetBool(AnimatorParams.DEATH, true);
        }

        myCollider.enabled = false;

        FieldActivity field = FieldManager.instance.fields[(int)myField];
        field.monsters.Remove(this);

        yield return new WaitForSeconds(0.2f);

        myCollider.enabled = true;

        ItemDrop();
        ReCycle();
        Disappear();
    }

    protected void ItemDrop()
    {
        for (int i = 0; i < dropItemCount; i++)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/FieldObject/DropItem");
           
            GameObject itemObject = PoolManager.instance.Spawn(prefab, myObject.position, Vector3.one, Quaternion.identity, true, myObject.parent);

            DropItem dropItem = itemObject.GetComponent<DropItem>();

            dropItem.moneyType = GameMoney.GameMoneyType.RUBY;
            dropItem.dropCount = 1;

            Vector3 dropPos = GetRandomPositionInBox(myObject.position, dropRange);

            dropItem.Drop_Animation(myObject.position, dropPos);
        }
    }


    Vector3 GetRandomPositionInBox(Vector3 center, Vector3 range)
    {
        float randomX = Random.Range(center.x - range.x / 2f, center.x + range.x / 2f);
        float randomY = Random.Range(center.y - range.y / 2f, center.y + range.y / 2f);

        return new Vector3(randomX, randomY, center.z);
    }
}
