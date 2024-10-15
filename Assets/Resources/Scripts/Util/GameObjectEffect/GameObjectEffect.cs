using GameSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "GameObject_Effect", menuName = "Effects/Effect")]
public class GameObjectEffect : ScriptableObject
{
    public enum EffectType
    {
        FloatingAndFadeout = 0,
        FloatingAndDestroy = 1,
    }

    public EffectType effectType;
    public GameObject target;
    public TextMeshPro text;

    public void Effect()
    {
        switch(effectType)
        {
            case EffectType.FloatingAndFadeout:
                GameManager.instance.effectManager.FloatingAndFadeOut(target, text);
                break;
            case EffectType.FloatingAndDestroy:
                GameManager.instance.effectManager.FloatingAndDestroy(target);
                break;
        }
        
    }
}
