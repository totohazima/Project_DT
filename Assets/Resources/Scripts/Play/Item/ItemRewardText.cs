using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemRewardText : FloatingText
{
    //public override void OnEnable()
    //{
    //    base.OnEnable();
    //}

    public void TextSetting(GameMoney.GameMoneyType type, int count)
    {
        string typeTxt = "";
        switch(type)
        {
            case GameMoney.GameMoneyType.GOLD:
                typeTxt = TextParams.GOLD;
                break;
            case GameMoney.GameMoneyType.RUBY:
                typeTxt = TextParams.RUBY;
                break;
        }

        string applyText = typeTxt + " +" + count;
        text.text = applyText;
        
        StartCoroutine(Text_Animation());
    }

    public override IEnumerator Text_Animation()
    {

        Vector3 destination = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        Vector3 fromPos = new Vector3(destination.x, destination.y - 0.2f, destination.z);

        LeanTween.value(text.gameObject, 1, 0, 1f).setOnUpdate(UpdateTextAlpha);
        LeanTween.move(gameObject, destination, 1f).setEase(LeanTweenType.easeOutSine).setFrom(fromPos);

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
