using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using StatusHelper;
using GameSystem;
using Pathfinding;
using System;

public class Character : FieldObject
{
    [HideInInspector] public Transform getTransform;
    public WhiteFlash whiteFlash;
    public SpriteRenderer viewSprite;
    public bool isDisable; //true일 경우 정지(모든 메서드)
    public bool isReadyToMove; //true일 경우 움직임
    public bool isReadyToAttack; //true일 경우 공격 가능
    public bool isMove;
    public bool isDead;
    private float attackTimer;
    [Header("StatusInfo")]
    public StatusInfo statusInfo;
    public PlayStatus playStatus;
    public Animator anim;
    public AILerp aiPath;
    [Header("TargetInfo")]
    public Transform targetUnit;  //타겟으로 잡힌 유닛
    public Vector3 targetLocation;  //이동해야 할 좌표(위치)

    public virtual void Start()
    {
        getTransform = transform;
        aiPath = GetComponent<AILerp>();
    }

    public void OnEnable()
    {
        playStatus.CurHealth = playStatus.MaxHealth;
        isReadyToMove = true;
    }

    public virtual void Update()
    {       
    }

    //유닛 이동
    public virtual void MoveUnit(Vector3 dir)
    {   
    }
    public virtual void AnimatonUpdate()
    {
    }

    /// <summary>
    /// 현재 스테이터스(상태, 스탯) 매 업데이트마다 설정
    /// </summary>
    public virtual void StatusUpdate()
    {
    }
    /// <summary>
    /// 공격이 들어오는지 체크하는 함수
    /// </summary>
    public virtual void OnHit(double damage)
    {
        DamageCalc(damage);
    }

    /// <summary>
    /// 공격이 들어온 경우 어느정도의 대미지가 들어오는지 계산하는 함수
    /// </summary>
    public virtual void DamageCalc(double damage)
    {
        playStatus.CurHealth -= damage;


        if (playStatus.CurHealth <= 0)
        {
            isDead = true;
            StartCoroutine(Death());
        }
        else
        {
            if (whiteFlash != null)
            {
                whiteFlash.PlayFlash();
            }
        }
    }

    /// <summary>
    /// ViewRange 내에 오브젝트 탐지
    /// </summary>
    public virtual void ObjectScan()
    { 
    }

    public virtual IEnumerator Death()
    {
        myCollider.enabled = false;
        attackTimer = 0f;
        isReadyToMove = false;
        isReadyToAttack = false;

        yield return new WaitForSeconds(1f);

        myCollider.enabled = true;
        PoolManager.instance.FalsedPrefab(gameObject, gameObject.name);
    }

#if UNITY_EDITOR
    int segments = 100;
    bool drawWhenSelected = true;

    void OnDrawGizmosSelected()
    {
        if (drawWhenSelected && getTransform != null)
        {
            //탐지 시야
            Gizmos.color = Color.cyan;
            DrawHollowCircle(getTransform.position, (float)playStatus.viewRange, segments);

            //공격 사거리
            Gizmos.color = Color.red;
            DrawHollowCircle(getTransform.position, (float)playStatus.attackRange, segments);
        }
    }

    void DrawHollowCircle(Vector3 center, float radius, int segments)
    {
        float angle = 0f;
        Vector3 lastPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);

        for (int i = 1; i <= segments; i++)
        {
            angle = i * Mathf.PI * 2f / segments;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
            Gizmos.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
        }
    }
#endif
}
