using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupItem : FloatingText
{
    public SpriteRenderer itemIcon;
    [Header("ItemInfo")]
    public GameMoney.GameMoneyType itemType;
    public int itemCount = 0;
    [SerializeField] private List<Sprite> itemSprite = new List<Sprite>();

    public Building target;

    public void ItemSetting(GameMoney.GameMoneyType type, int count)
    {
        Sprite typeImage = null;
        typeImage = itemSprite[(int)type];

        //itemIcon.sprite = typeImage;
        StartCoroutine(Text_Animation());
    }

    public override IEnumerator Text_Animation()
    {
        Vector3 destination = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        Vector3 fromPos = new Vector3(destination.x, destination.y - 0.3f, destination.z);

        LeanTween.value(itemIcon.gameObject, 0, 1, 0.3f).setOnUpdate(UpdateTextAlpha);
        LeanTween.move(gameObject, destination, 0.3f).setEase(LeanTweenType.easeOutSine).setFrom(fromPos);

        yield return new WaitForSeconds(0.2f);

        float delay = 0.4f;
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = target.interactionCenter.position;
        Vector3 middlePos = CalculateControlPoint(startPos, endPos, 0.3f);

        while (elapsedTime < delay)
        {
            float t = elapsedTime / delay;
            Vector3 point = CalculateQuadraticBezierPoint(t, startPos, middlePos, endPos);

            transform.position = point;

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.position = endPos;
        target.RewardPopup();

        Recycle();
        Disappeer();
    }

    protected override void Recycle()
    {
        //itemIcon.sprite = null;
        target = null;
    }
    private void UpdateTextAlpha(float alpha)
    {
        Color color = itemIcon.color;
        color.a = alpha;
        itemIcon.color = color;
    }

    public override void Disappeer()
    {
        base.Disappeer();
    }

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }

    /// <summary>
    /// �������� ����, � ���̸� �Է¹޾� �������� ����ϴ� �Լ�
    /// </summary>
    Vector3 CalculateControlPoint(Vector3 startPoint, Vector3 endPoint, float curveHeight)
    {
        // �� ���� �߰� ������ ���
        Vector3 middlePoint = (startPoint + endPoint) / 2f;

        // �߰� �������� ���� �Ʒ��� �������� �־� ��� ���� (� ���� ����)
        middlePoint.y += curveHeight; // y������ �־����� ���

        return middlePoint; // ������ ��ȯ
    }
}
