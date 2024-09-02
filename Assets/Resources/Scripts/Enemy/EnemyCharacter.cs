using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    public override void Update()
    {
        if(isDead)
        {
            StartCoroutine(Death());
        }
    }

    public override IEnumerator Death()
    {
        anim.SetBool("Dead", true);
        return base.Death();
    }
}
