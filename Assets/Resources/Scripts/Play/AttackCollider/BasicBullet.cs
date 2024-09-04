using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBullet : AttackCollider
{
    public enum Type
    {
        BULLET,
        AREA,
        LASER,
        GUIDED,
    }
    public Type type;
    public Vector3 destination;
    protected Vector3 direction;
    public float speed = 1f;
    private void Start()
    {
        SetBulletTarget(target.transform.position);  
    }
    public void SetBulletTarget(Vector3 target)
    {
        type = Type.BULLET;
        SetDestination(target);
        //SetRotation(target);
    }
    
    public virtual void Update()
    {
        UpdateBullet();
    }

    void UpdateBullet()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    protected void SetDestination(Vector3 destination_T)
    {
        destination_T.y = transform.position.y;
        destination = destination_T;
        direction = (destination - transform.position).normalized;
    }

    protected void SetRotation(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = rot;
    }
}
