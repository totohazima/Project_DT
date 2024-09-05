using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    [Header("RandomMove_Info")]
    //private float randomMoveRadius = 2f; //이 만큼의 거리내로 랜덤 이동
    private float randomMoveTime; //이 시간 동안 타겟이 잡히지 않으면 일정 거리 내 위치로 랜덤 이동
    private float randomMoveTime_Max = 5f;
    private float randomMoveTime_Min = 1f;
    [Header("ScanTime_Info")]
    private float scanDelay = 0.1f; //스캔이 재작동하는 시간
    private bool isScanning = false; //스캔 코루틴이 실행중인지 체크하는 변수

    public override void Update()
    {
        RandomMoveLocation();
        StartCoroutine(ObjectScan(scanDelay));
        StatusUpdate();
        AnimationUpdate();
    }
    private void RandomMoveLocation()
    {
        
    }
    public override IEnumerator ObjectScan(float scanDelay)
    {
        if (!isScanning)
        {
            isScanning = true;

            yield return new WaitForSeconds(scanDelay);

            Collider[] detectedColls = Physics.OverlapSphere(myObject.position, (float)playStatus.viewRange, 1 << 6);
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
            }

            isScanning = false;
        }
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

        //스탯
        aiPath.speed = (float)playStatus.MoveSpeed;
    }

    public override void AnimationUpdate()
    {
        if (isMove)
        {
            if (anim.GetBool("Move") == false)
            {
                anim.SetBool("Move", true);
            }

            if (aiPath.steeringTarget.x < myObject.position.x) //왼쪽
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
            if (anim.GetBool("Move") == true)
            {
                anim.SetBool("Move", false);
            }
        }
    }

    public override IEnumerator Death()
    {
        if (!anim.GetBool("Dead"))
        {
            anim.SetBool("Dead", true);
        }

        return base.Death();
    }
}
