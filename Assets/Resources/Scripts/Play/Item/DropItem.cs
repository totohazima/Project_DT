using GameSystem;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropItem : FieldObject, IPointerClickHandler
{
    [Header("ItemInfo")]
    public GameMoney.GameMoneyType moneyType;
    public int dropCount = 1;
    [SerializeField] private bool readyToGet = false; //true�� ��� ���� ����
    private GameObject rewardText = null;
    [Header("DestroyInfo")]
    public float destroyTime = 60f;

    public void Drop_Animation(Vector3 startPos, Vector3 endPos)
    {
        StartCoroutine(AutoDisappear(destroyTime));
        StartCoroutine(Animation(startPos, endPos));
    }

    // �ڷ�ƾ���� �ݺ� �۾��� Public���� �ٸ� ��ũ��Ʈ���� �����ϰ� �ϸ� �ȵȴ�. �ݵ�� �ش� ��ũ��Ʈ������ ��ü������ �����ϰ� �ؾ� ��.
    private IEnumerator Animation(Vector3 startPos, Vector3 endPos)
    {
        float delay = 0.3f;
        float elapsedTime = 0f;
        Vector3 middlePos = CalculateControlPoint(startPos, endPos, 0.5f);

        while (elapsedTime < delay)
        {
            float t = elapsedTime / delay;
            Vector3 point = CalculateQuadraticBezierPoint(t, startPos, middlePos, endPos);

            transform.position = point;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = endPos;
        readyToGet = true;
    }
    
    void Awake()
    {
        rewardText = Resources.Load<GameObject>("Prefabs/FieldObject/ItemRewardTxt");
    }

    private void Recycle()
    {
        moneyType = GameMoney.GameMoneyType.GOLD;
        dropCount = 1;
        readyToGet = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (readyToGet)
        {
            GetItem();
        }
    }
    protected IEnumerator AutoDisappear(float timer)
    {
        yield return new WaitForSeconds(timer); //�ð� ���� �� �ڵ����� ����

        Disappear();
    }

    protected void GetItem()
    {
        StopCoroutine(AutoDisappear(destroyTime)); //�ڷ�ƾ ����

        PlayerInfo playerInfo = GameManager.instance.gameDataBase.playerInfo;

        switch(moneyType)
        {
            case GameMoney.GameMoneyType.GOLD:
                playerInfo.gold += dropCount;
                break;
            case GameMoney.GameMoneyType.RUBY:
                playerInfo.ruby += dropCount;
                break;
        }

        ShowFloatingText();
        Recycle();
        Disappear();
    }

    
    protected void ShowFloatingText()
    {
        GameObject text = PoolManager.instance.Spawn(rewardText, myObject.position, Vector3.one, Quaternion.identity, true, myObject.parent);

        ItemRewardText floatText = text.GetComponent<ItemRewardText>();
        floatText.TextSetting(moneyType, dropCount);
    }
    protected void Disappear()
    {
        PoolManager.instance.Release(gameObject);    
    }

    /// <summary>
    /// ������ ����� Ư�� ������ ��ǥ�� ����ϴ� �Լ�
    /// </summary>
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
