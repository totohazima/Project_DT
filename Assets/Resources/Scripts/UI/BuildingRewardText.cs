using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingRewardText : FloatingText
{
    private Vector3 startPosition;
    public GameObjectEffect effect;
    public void TextSetting(GameMoney.GameMoneyType type, int count, Vector3 startPos)
    {
        string typeTxt = "";
        typeTxt = type.ToString();

        var field = typeof(TextParams).GetField(typeTxt);
        if(field != null)
        {
            typeTxt = field.GetValue(null) as string;
        }

        string applyText = typeTxt + " +" + count;
        text.text = applyText;

        startPosition = startPos;

        effect.target = gameObject;
        effect.text = text;
        effect.Effect();
        //StartCoroutine(Text_Animation());
    }

    public override IEnumerator Text_Animation()
    {

        Vector3 destination = new Vector3(startPosition.x, startPosition.y + 0.2f, startPosition.z);
        Vector3 fromPos = new Vector3(destination.x, destination.y - 0.2f, destination.z);

        LeanTween.value(text.gameObject, 1, 0, 1f).setOnUpdate(UpdateTextAlpha);
        LeanTween.move(gameObject, destination, 1f).setEase(LeanTweenType.easeOutSine).setFrom(fromPos);

        yield return new WaitForSeconds(1f);

        Recycle();
        Disappeer();
    }

    private void UpdateTextAlpha(float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}
