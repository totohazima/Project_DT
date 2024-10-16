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
    [SerializeField] private bool readyToGet = false; //true인 경우 습득 가능
    private GameObject rewardText = null;
    [Header("DestroyInfo")]
    public float destroyTime = 60f;

    public void Drop_Animation(Vector3 startPos, Vector3 endPos)
    {
        StartCoroutine(AutoDisappear(destroyTime));
        StartCoroutine(Animation(startPos, endPos));
    }

    // 코루틴으로 반복 작업시 Public으로 다른 스크립트에서 실행하게 하면 안된다. 반드시 해당 스크립트내에서 자체적으로 실행하게 해야 함.
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
        yield return new WaitForSeconds(timer); //시간 지날 시 자동으로 삭제

        Disappear();
    }

    protected void GetItem()
    {
        StopCoroutine(AutoDisappear(destroyTime)); //코루틴 해제

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
    /// 베지에 곡선에서 특정 시점의 좌표를 계산하는 함수
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
    /// 시작점과 끝점, 곡선 높이를 입력받아 제어점을 계산하는 함수
    /// </summary>
    Vector3 CalculateControlPoint(Vector3 startPoint, Vector3 endPoint, float curveHeight)
    {
        // 두 점의 중간 지점을 계산
        Vector3 middlePoint = (startPoint + endPoint) / 2f;

        // 중간 지점에서 위나 아래로 오프셋을 주어 곡선을 만듦 (곡선 높이 적용)
        middlePoint.y += curveHeight; // y축으로 휘어지는 경우

        return middlePoint; // 제어점 반환
    }

}
