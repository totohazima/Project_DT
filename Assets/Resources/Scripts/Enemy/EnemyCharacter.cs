using FieldHelper;
using GameEvent;
using GameSystem;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class EnemyCharacter : Character
{
    [Header("Field and Movement Info")]
    private bool onRandomMove = false;
    private float randomMoveTime; //�� �ð� ���� Ÿ���� ������ ������ ���� �Ÿ� �� ��ġ�� ���� �̵�
    private float randomMoveTime_Max = 5f;
    private float randomMoveTime_Min = 1f;
    [Header("Scan_Info")]
    public List<HeroCharacter> soonAttacker = new List<HeroCharacter>(); //�� ���͸� Ÿ������ ���� ����
    public int soonAttackerLimit; //-1�̸� Ÿ�� ���� ����
    private float scanDelay = 0.1f; //��ĵ�� ���۵��ϴ� �ð�
    private bool isScanning = false; //��ĵ �ڷ�ƾ�� ���������� üũ�ϴ� ����
    [Header("ItemDrop_Info")]
    public int minDropCount;
    public int maxDropCount; //������ �� ��� ����
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
        soonAttacker.Clear();
        playStatus_Desire.Reset();
        playStatus_KDA.Reset();

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

        StartCoroutine(RandomMoveLocation(currentField));
        //StartCoroutine(ObjectScan(scanDelay));
        AttackRangeScan();
        StatCalculate();
        StatusUpdate();
        AnimationUpdate(); 
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
        // ������ �ڽ� ������ ������ ��ġ ����
        Vector3 randomPositionWithinBox = new Vector3(
            Random.Range(-boxSize.x / 2, boxSize.x / 2),
            Random.Range(-boxSize.y / 2, boxSize.y / 2),
            Random.Range(-boxSize.z / 2, boxSize.z / 2)
        );

        // ���� ��ġ�� ���� ������� ��ġ�� �����Ͽ� �̵�
        FieldActivity controlField = FieldManager.instance.fields[(int)field];
        targetLocation = controlField.getTransform.position + randomPositionWithinBox;

        onRandomMove = false;  // �̵� ����
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
        else if (targetField != currentField)
        {
            Vector3 fieldPos = Vector3.zero;

            SetTargetPosition(fieldPos);
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
        {
            isMove = false;
        }

        if (soonAttackerLimit != -1)
        {
            if (soonAttacker.Count > soonAttackerLimit)
            {
                isUntargetted = true;
            }
            else
            {
                isUntargetted = false;
            }
        }
        else
        {
            isUntargetted = false;
        }
    }
    private void UpdateLerpSpeed()
    {
        aiPath.canMove = isMove;
        aiPath.maxSpeed = stateController.moveSpeed;

        if (aiPath.reachedDestination || aiPath.reachedEndOfPath)
        {
            isMove = false;
        }
    }
    private void SetTargetPosition(Vector3 targetPosition)
    {
        isMove = true;
        aiPath.destination = new Vector3(targetPosition.x, targetPosition.y, 0);
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
            if (aiPath.steeringTarget.x < myObject.position.x)
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

    public override void OnHit(double damage, Character attacker)
    {
        base.OnHit(damage, attacker);

        //�ǰ� ������ Ÿ��
        targetUnit = attacker.transform;
    }

    public override IEnumerator Death()
    {
        if (!anim.GetBool(AnimatorParams.DEATH))
        {
            anim.SetBool(AnimatorParams.DEATH, true);
        }

        myCollider.enabled = false;

        FieldActivity field = FieldManager.instance.fields[(int)currentField];
        if(field.monsters.Contains(this))
        {
            field.monsters.Remove(this);
            field.bossPoint += Random.Range(1, 4);
        }
        else if(field.bosses.Contains(this))
        {
            field.bosses.Remove(this);
            field.isBossSpawned = false;
            field.HeroEliteCombatCalc(false);
        }
        
        yield return new WaitForSeconds(0.1f);

        myCollider.enabled = true;

        ItemDrop();
        ReCycle();
        Disappear();
    }

    protected void ItemDrop()
    {
        int count = Random.Range(minDropCount, maxDropCount + 1);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/FieldObject/DropItem");
           
            GameObject itemObject = PoolManager.instance.Spawn(prefab, myObject.position, Vector3.one, Quaternion.identity, true, myObject.parent);

            DropItem dropItem = itemObject.GetComponent<DropItem>();

            dropItem.moneyType = GameMoney.GameMoneyType.RUBY;
            dropItem.dropCount = 1;

            Vector3 dropPos = GetRandomPositionInBox(dropCenter.position, dropRange);

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
