using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public new Transform transform;
    public Collider attackCollider;
    public Character owner;
    public Character target;
    public float lookAtDir;

    private void Awake()
    {
        transform = gameObject.transform;
    }

    protected virtual void OnTriggerEnter(Collider collision)
    {
        if(collision == null)
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
        onHItCharacter.OnHit(owner.playStatus.attackPower);
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
}
