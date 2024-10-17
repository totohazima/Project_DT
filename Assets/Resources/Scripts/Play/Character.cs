using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using StatusHelper;
using FieldHelper;
using GameSystem;
using Pathfinding;
using System;
using Util;
using Unity.Mathematics;
using UnityEngine;

public class Character : FieldObject, ICustomUpdateMono
{
    public WhiteFlash whiteFlash;
    public StateController stateController;
    public PopupController popupController;
    public UICharacterCostume characterCostume;
    public SpriteGroup spriteGroup;
    public Transform popupCenter;
    public Transform dropCenter;
    public bool isInvincible; //true�� ��� ����
    public bool isUntargetted; //true�� ��� Ÿ������ ������ ����
    public bool isStopScanning; //true�� ��� ��ĵ ����
    public bool isReadyToMove; //true�� ��� ������
    public bool isReadyToAttack; //true�� ��� ���� ����
    public bool isMove;
    public bool isAttacking;
    public bool isDead;

    [Header("StatusInfo")]
    public FieldMap.Field currentField;
    public string code;
    public string characterName;
    public string jobClass;
    public PlayStatus playStatus;
    public Desire_PlayStatus playStatus_Desire;
    public KDA_PlayStatus playStatus_KDA;
    public Animator anim;
    public AIPath aiPath;
    public Vector3 dropRange = new Vector3(1f, 1f, 1f);

    [Header("TargetInfo")]
    public FieldMap.Field targetField; //���� ���� �� �ʵ�
    public Transform targetUnit;  //Ÿ������ ���� ����
    public Vector3 targetLocation;  //�̵��ؾ� �� ��ǥ(��ġ)
    public Building.BuildingType targetBuilding; //���� ���� �� �ǹ�
    protected bool onTargetFieldPos = false; //�ʵ� �� ������ġ�� ���� �� true��
    protected bool onTargetBuildingPos = false; //�ǹ� ���� ������ġ ���� �� true��

    public void OnEnable()
    {
        CustomUpdateManager.customUpdateMonos.Add(this);
        playStatus.CurHealth = playStatus.MaxHealth;
        ReCycle();
        isReadyToMove = true;
    }
    public void OnDisable()
    {
        CustomUpdateManager.customUpdateMonos.Remove(this);   
    }
    public virtual void ReCycle()
    {
        isReadyToAttack = false;
        isAttacking = false;
        isReadyToMove = false;
        isMove = false;
        isDead = false;

        aiPath.destination = myObject.position;
        targetUnit = null;
        targetLocation = Vector3.zero;
        targetBuilding = Building.BuildingType.NONE;

        StopAllCoroutines();
    }
    public virtual void CustomUpdate()
    {       
    }

    //���� �̵�
    public virtual void MoveUnit(Vector3 dir)
    {   
    }

    /// <summary>
    /// ���� �������ͽ�(����, ����) �� ������Ʈ���� ����
    /// </summary>
    public virtual void StatusUpdate()
    {
    }
    public virtual void AnimationUpdate()
    {
    }

    /// <summary>
    /// ������ �������� üũ�ϴ� �Լ�
    /// </summary>
    public virtual void OnHit(double damage, Character attacker)
    {
        DamageCalc(damage, attacker);
    }

    /// <summary>
    /// ������ ���� ��� ��������� ������� �������� ����ϴ� �Լ�
    /// </summary>
    public virtual void DamageCalc(double damage, Character attacker)
    {
        if (isInvincible)
        {
            damage = 0f;
        }

        playStatus.CurHealth -= damage;


        if (playStatus.CurHealth <= 0)
        {
            if (attacker.gameObject.layer == LayerMask.NameToLayer(Layers.Player))
            {
                List<GameMoney.GameMoneyType> types = new List<GameMoney.GameMoneyType>();
                List<int> counts = new List<int>();

                types.Add(GameMoney.GameMoneyType.GOLD);
                int point = UnityEngine.Random.Range(1, 4);
                counts.Add(point);
                attacker.popupController.ItemPopup(types, counts);
            }

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

    public virtual void StatCalculate()
    {
        if(stateController == null)
        {
            return;
        }
        
        stateController.SetMoveSpeedPercent(playStatus.moveSpeedPercent);
        stateController.SetAttackSpeedPercent(playStatus.attackSpeedPercent);
    }

    /// <summary>
    /// ViewRange ���� ������Ʈ Ž��
    /// </summary>
    public virtual IEnumerator ObjectScan(float scanDelay)
    {
        yield return null;
    }

    public virtual IEnumerator Death()
    {
        yield return 0;
    }

    public virtual void Disappear()
    {
        PoolManager.instance.Release(gameObject);
    }
    public void Push(Vector3 vector, bool ignoreKinematic = false)
    {
        rigid.isKinematic = ignoreKinematic;
        rigid.AddForce(vector, ForceMode.Impulse);

        rigid.isKinematic = true;
    }

#if UNITY_EDITOR
    int segments = 100;
    bool drawWhenSelected = true;

    void OnDrawGizmosSelected()
    {
        if (drawWhenSelected && myObject != null)
        {
            //Ž�� �þ�
            Gizmos.color = Color.cyan;
            DrawHollowCircle(myObject.position, (float)playStatus.viewRange, segments);

            //���� ��Ÿ�
            Gizmos.color = Color.red;
            DrawHollowCircle(myObject.position, (float)playStatus.attackRange, segments);

            if (dropCenter != null)
            {
                //������ ��� ����
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(dropCenter.position, dropRange);
            }
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
