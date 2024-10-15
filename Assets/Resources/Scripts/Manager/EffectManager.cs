using GameSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectManager : MonoBehaviour
{

    //올라가면서 서서히 사라짐
    public void FloatingAndFadeOut(GameObject target, TextMeshPro text)
    {
        StartCoroutine(FloatingAndFadeOut_Anim(target, text));

        IEnumerator FloatingAndFadeOut_Anim(GameObject target, TextMeshPro text)
        {
            Vector3 destination = new Vector3(target.transform.position.x, target.transform.position.y + 0.2f, transform.position.z);
            Vector3 fromPos = new Vector3(destination.x, destination.y - 0.2f, destination.z);

            LeanTween.value(target, 1, 0, 1f).setOnUpdate(alpha => UpdateTextAlpha(alpha, text));
            LeanTween.move(target, destination, 1f).setEase(LeanTweenType.easeOutSine).setFrom(fromPos);

            yield return new WaitForSeconds(1f);

            PoolManager.instance.Release(target);
        }

        void UpdateTextAlpha(float alpha, TextMeshPro text)
        {
            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }
    }

    //올라가고 사라짐
    public void FloatingAndDestroy(GameObject target)
    {
        StartCoroutine(FloatingAndDestroy_Anim(target));

        IEnumerator FloatingAndDestroy_Anim(GameObject target)
        {
            Vector3 destination = new Vector3(target.transform.position.x, target.transform.position.y + 0.15f, transform.position.z);
            Vector3 fromPos = new Vector3(destination.x, destination.y - 0.15f, destination.z);

            LeanTween.move(target, destination, 0.7f).setEase(LeanTweenType.easeOutBack).setFrom(fromPos);

            yield return new WaitForSeconds(0.7f);

            PoolManager.instance.Release(target);
        }
    }
}
