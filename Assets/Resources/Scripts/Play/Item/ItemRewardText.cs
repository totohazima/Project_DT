using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemRewardText : FloatingText
{
    public override void OnEnable()
    {
        base.OnEnable();
    }

    public void TextSetting(GameMoney.GameMoneyType type, int count)
    {
        string typeTxt = "";
        switch(type)
        {
            case GameMoney.GameMoneyType.GOLD:
                typeTxt = "°ñµå";
                break;
            case GameMoney.GameMoneyType.RUBY:
                typeTxt = "·çºñ";
                break;
        }

        string applyText = typeTxt + " +" + count;
        text.text = applyText;
    }

    public override IEnumerator Text_Animation()
    {
        Vector3 destination = new Vector3(parentTransform.position.x, parentTransform.position.y + 0.2f, parentTransform.position.z);

        LeanTween.value(text.gameObject, 1, 0, 1f).setOnUpdate(UpdateTextAlpha);
        LeanTween.move(gameObject, destination, 1f).setEase(LeanTweenType.easeOutSine).setFrom(parentTransform.position);

        yield return new WaitForSeconds(1f);

        Disappeer();
    }

    private void UpdateTextAlpha(float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }

    public override void Disappeer()
    {
        base.Disappeer();
    }
}
