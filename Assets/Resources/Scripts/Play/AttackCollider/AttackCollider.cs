using StatusHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using GameSystem;

public class AttackCollider : MonoBehaviour
{
    public new Transform transform;
    public Collider attackCollider;

    public Character owner;
    public Character target;

    [Header("Push")]
    public bool usePush = false;
    public Vector3 pushVector = Vector3.zero;

    [Header("몇명에게 Hit 판정 -1이면 무시")]
    public int HitCount = -1;
    protected int runHitCount = -1;

    [Header("SingleHit (단일히트, 지정대상 한명)")]
    public bool isSingleHit = false;

    [Header("ManyHit (다단히트)")]
    public bool isManyHit = false; // 다단히트
    public float manyHitWaitTime = 1.0f;
    Coroutine coManyHit = null;

    [Header("반드시 죽음")]
    /// <summary>
    /// 반드시 죽음
    /// </summary>
    public bool isSurelyDeath = false;

    [Header("추가 스탯")]
    public double attackDamage;
    public PlayStatus status;

    public float lookAtDir;

    private void Awake()
    {
        transform = gameObject.transform;
    }
    private void OnEnable()
    {
        //AttackColliderManager.managedAttackColliderList.Add(this);
        if (attackCollider == null)
        {
            enabled = true;
            return;
        }
        if (isManyHit == true)
        {
            if (coManyHit != null)
            {
                StopCoroutine(coManyHit);
                coManyHit = null;
            }
            //coManyHit = StartCoroutine(DoManyHit());
        }
    }

    private void OnDisable()
    {
        //AttackColliderManager.managedAttackColliderList.Remove(this);
    }

    public virtual void Recycle(Character _owner, Character _target)
    {
        owner = _owner;
        target = _target;

        if (owner.gameObject.layer == LayerMask.NameToLayer(Layers.Player))
        {
            gameObject.layer = LayerMask.NameToLayer(Layers.PlayerAttackCollider);
        }
        else if (owner.gameObject.layer == LayerMask.NameToLayer(Layers.Enemy))
        {
            gameObject.layer = LayerMask.NameToLayer(Layers.EnemyAttackCollider);
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer(Layers.PlayerAttackCollider);
        }
    }
    protected virtual void OnTriggerEnter(Collider collision)
    {
        if(collision == null && collision.gameObject.layer == owner.gameObject.layer)
        {
            return;
        }
        Character onHitCharacter = collision.GetComponentInParent<Character>();
        if(onHitCharacter == null || onHitCharacter.isDead == true)
        {
            return;
        }

        OnHit(onHitCharacter);

    }

    public virtual void OnHit(Character onHItCharacter)
    {
        if (onHItCharacter == null || onHItCharacter.isDead == true)
            return;
        if (runHitCount <= 0 && HitCount != -1)
            return;

        // 단일 히트이지만, 대상이 아니다
        if (isSingleHit == true && target != null && target.Equals(onHItCharacter) == false)
            return;

        if (false == (onHItCharacter is HeroCharacter))
        {
            //SoundManager.Instance.SFXPlay(SFXPack);
        }

        if (isSurelyDeath == false)
        {
            OnHitAction(onHItCharacter);
        }

        if (usePush == true)
            onHItCharacter.Push(pushVector * lookAtDir);

        if (isSurelyDeath == true)
        {
            // 일반 몬스터가 아닌 몬스터는 제외
            StartCoroutine(onHItCharacter.Death());
        }
        double damage = attackDamage;
        onHItCharacter.OnHit(damage, owner);

        if (HitCount != -1)
            runHitCount--;
        if (runHitCount <= 0 && HitCount != -1)
            Disappear();
        if (isSingleHit && HitCount == -1)
            Disappear();
    }

    protected virtual void OnHitAction(Character hittedCharacter)
    {
        DamageCalculate(hittedCharacter);

    }

    protected void DamageCalculate(Character hittedCharacter)
    {
        if(hittedCharacter == null)
        {
            return;
        }

        attackDamage = owner.playStatus.attackPower + status.attackPower;
    }
    public void LookAt(Vector3 target)
    {
        if(target.x - transform.position.x > 0.0f)
        {
            lookAtDir = 1;
            transform.eulerAngles = Vector3.zero;
        }
        else if(target.x - transform.position.x < 0.0f)
        {
            lookAtDir = -1;
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    public void SetLookAtEulerAngles(float angle)
    {
        angle = Mathf.Abs(angle);
        if (angle < 90.0f)
        {
            lookAtDir = 1F;
            transform.eulerAngles = new Vector3(0.0f, 0, 0.0f);
        }
        else
        {
            lookAtDir = -1F;
            transform.eulerAngles = new Vector3(0.0f, 180, 0.0f);
        }
    }

    public virtual void Disappear()
    {
        enabled = false;
        if (coManyHit != null) StopCoroutine(coManyHit);
        coManyHit = null;

        owner = null;
        target = null;

        if (attackCollider != null)
            attackCollider.enabled = true;
        PoolManager.instance.Release(gameObject);
    }
}
