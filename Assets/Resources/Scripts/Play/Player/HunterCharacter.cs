using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HunterCharacter : Character, IPointerClickHandler
{
    public bool isFieldEnter = false;
    [Header("RandomMove_Info")]
    public bool onRandomMove = false;
    private float randomMoveRadius = 2f; //이 만큼의 거리내로 랜덤 이동
    private float randomMoveTime; //이 시간 동안 타겟이 잡히지 않으면 일정 거리 내 위치로 랜덤 이동
    private float randomMoveTime_Max = 10f; 
    private float randomMoveTime_Min = 3f;
    [Header("ScanTime_Info")]
    private float scanDelay = 0.1f; //스캔이 재작동하는 시간
    private bool isScanning = false; //스캔 코루틴이 실행중인지 체크하는 변수
    protected bool onClickProcess; //유닛 클릭 여부 체크 (연속 클릭 방지용)


    public override void Update()
    {
        if(isDisable)
        {
            return;
        }

        StartCoroutine(ObjectScan(scanDelay));
        StartCoroutine(RandomMoveLocation());
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

    private IEnumerator RandomMoveLocation()
    {
        if (!onRandomMove)
        {
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
        ///타겟 상태
        if (targetUnit != null)
        {
            isMove = true;
            aiPath.destination = targetUnit.position;
        }
        else if (targetField != null)
        {
            isMove = true;
            aiPath.destination = targetField.position;
        }
        else
        {
            //타겟이 잡히지 않을 경우
            if (targetLocation != Vector3.zero)
            {
                isMove = true;
                aiPath.destination = targetLocation;
            }
            else
            {
                isMove = false;
            }
        }

        ///공격 상태
        if (isReadyToAttack)
        {
            isMove = false;
        }
        else
        {
            isMove = true;
        }
         
        ///이동 상태
        if (isMove)
        {
            aiPath.canMove = true;

            if(targetField != null && myObject.position == targetField.position || targetUnit != null)
            {
                targetField = null;
            }

            //움직이는 중 목적지에 도달하거나 최대 경로에 도달한 경우
            if (aiPath.reachedDestination || aiPath.reachedEndOfPath)
            {
                isMove = false;
            }
        }
        else
        {
            aiPath.canMove = false;
        }

        //스탯
        aiPath.speed = (float)playStatus.MoveSpeed;
    }

    public override void AnimationUpdate()
    {
        ///이동 애니메이션
        if (isMove)  
        {
            if (!anim.GetBool("Move"))
            {
                anim.SetBool("Move", true);
            }

            if(aiPath.steeringTarget.x < myObject.position.x) //왼쪽
            {
                viewObject.rotation = Quaternion.Euler(0, 180, 0); 
            }
            else //오른쪽
            {
                viewObject.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            if (anim.GetBool("Move"))
            {
                anim.SetBool("Move", false);
            }
        }

        ///공격 애니메이션
        attackTimer += Time.deltaTime;
        if (isReadyToAttack && attackTimer >= playStatus.attackSpeed) 
        {
            if(!anim.GetBool("DevilAttack_01"))
            {
                anim.SetBool("DevilAttack_01", true);
                attackTimer = 0f;
            }
        }
        else
        {
            if (anim.GetBool("DevilAttack_01"))
            {
                anim.SetBool("DevilAttack_01", false);
            }
        }
    }

    public override IEnumerator Death(Character character)
    {
        anim.SetBool("Death", true);

        return base.Death(character);
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
                Debug.Log("유닛 추적 활성화");
            }
        }

        yield return 0;
    }

}
