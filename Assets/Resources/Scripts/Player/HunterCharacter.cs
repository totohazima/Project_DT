using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HunterCharacter : Character
{
    [Header("RandomMove_Info")]
    private float randomMoveRadius = 2f; //이 만큼의 거리내로 랜덤 이동
    private float randomMoveTime; //이 시간 동안 타겟이 잡히지 않으면 일정 거리 내 위치로 랜덤 이동
    private float randomMoveTime_Max = 5f; 
    private float randomMoveTime_Min = 1f;
    private float randomMoveTimer = 0f;
    [Header("ScanTime_Info")]
    private float scanDelay = 0.1f; //스캔이 재작동하는 시간
    private float scantimer = 0;

    public override void Update()
    {
        if(isDisable)
        {
            return;
        }

        scantimer += Time.deltaTime;
        if (scantimer > scanDelay)
        {
            ObjectScan();
        }
        RandomMoveLocation();
        StatusUpdate();
        AnimatonUpdate();

        // PC 환경에서 마우스 왼쪽 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ProcessClick(Input.mousePosition));
        }

        // 모바일 환경에서 터치 감지
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                StartCoroutine(ProcessClick(touch.position));
            }
        }
    }

    private void RandomMoveLocation()
    {
        if(randomMoveTime == 0f)
        {
            randomMoveTime = Random.Range(randomMoveTime_Min, randomMoveTime_Max);
        }

        if(targetUnit == null)
        {
            randomMoveTimer += Time.deltaTime;
            if(randomMoveTimer > randomMoveTime)
            {
                Vector3 randomLocation = Random.insideUnitCircle * randomMoveRadius;
                randomLocation.z = 0;

                Vector3 targetPos = getTransform.position + randomLocation;
                targetLocation = targetPos;

                randomMoveTimer = 0f;
                randomMoveTime = Random.Range(randomMoveTime_Min, randomMoveTime_Max);
            }
        }
        else
        {
            randomMoveTimer = 0f;
            targetLocation = Vector3.zero;
        }
    }
    public override void ObjectScan()
    {
        Collider[] detectedColls = Physics.OverlapSphere(getTransform.position, (float)playStatus.viewRange, 1 << 6);
        float shortestDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (Collider col in detectedColls)
        {
            if (col == null || col == myCollider)
            {
                continue;
            }

            Transform target = col.transform;
            float dis = Vector3.Distance(getTransform.position, target.position);

            if(dis < shortestDistance)
            {
                shortestDistance = dis;
                nearestTarget = target;
            }
        }

        if(nearestTarget != null)
        {
            targetUnit = nearestTarget;

        }
        else
        {
            targetUnit = null;
        }

        scantimer = 0f;
    }

    public override void StatusUpdate()
    {
        //상태
        if (targetUnit != null)
        {
            isMove = true;
            aiPath.destination = targetUnit.position;
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

        if (isMove)
        {
            aiPath.canMove = true;

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

        if (isDead)
        {
            StartCoroutine(Death());
        }

        //스탯
        aiPath.maxSpeed = (float)playStatus.MoveSpeed;
    }

    public override void AnimatonUpdate()
    {
        if(isMove)
        {
            anim.SetBool("Run", true);

            if(aiPath.steeringTarget.x < getTransform.position.x)
            {
                viewSprite.flipX = true;
            }
            else
            {
                viewSprite.flipX = false;
            }
        }
        else
        {
            anim.SetBool("Run", false);
        }
    }
    public override IEnumerator Death()
    {
        anim.SetBool("Dead", true);
        return base.Death();
    }

    /// <summary>
    /// 캐릭터 클릭 시 추적
    /// </summary>
    public IEnumerator ProcessClick(Vector3 clickPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(clickPosition);
        RaycastHit hit;

        FieldActivity.instance.cameraDrag.isCameraMove = false;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == myCollider)
            {
                FieldActivity.instance.cameraDrag.trackingTarget = this;
                Debug.Log("유닛 추적 활성화");
            }
            else
            {
                FieldActivity.instance.cameraDrag.trackingTarget = null;
                Debug.Log("유닛 추적 비 활성화");
            }
        }
        else
        {
            FieldActivity.instance.cameraDrag.trackingTarget = null;
            Debug.Log("유닛 추적 비 활성화");
        }

        //딜레이를 주지 않으면 CameraDrag 스크립트에서 카메라를 움직여버림
        yield return new WaitForSeconds(0.5f);

        FieldActivity.instance.cameraDrag.isCameraMove = true;
    }
  
}
