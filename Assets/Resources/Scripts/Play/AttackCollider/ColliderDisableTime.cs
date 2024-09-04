using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDisableTime : MonoBehaviour
{
    [SerializeField]
    float disableTime = 0F;
    [SerializeField, ReadOnly]
    float playDisableTime = 0F;
    public float PlayDisableTime { get { return playDisableTime; } }
    [SerializeField]
    Collider targetCollider = null;

    Coroutine coWaitForTime = null;

    void Awake()
    {
        playDisableTime = disableTime;
    }

    void OnDestroy()
    {
        if (coWaitForTime != null)
            StopCoroutine(coWaitForTime);
    }

    void OnEnable()
    {
        coWaitForTime = StartCoroutine(DoWaitForTime());
    }
    void OnDisable()
    {
        targetCollider.enabled = true;
        if (coWaitForTime != null)
            StopCoroutine(coWaitForTime);
    }

    public void ReStart(float _disableTime)
    {
        if (coWaitForTime != null)
            StopCoroutine(coWaitForTime);

        playDisableTime = _disableTime;
        coWaitForTime = StartCoroutine(DoWaitForTime());
    }

    IEnumerator DoWaitForTime()
    {
        yield return new WaitForSeconds(playDisableTime);
        playDisableTime = disableTime;
        targetCollider.enabled = false;
        coWaitForTime = null;
    }
}