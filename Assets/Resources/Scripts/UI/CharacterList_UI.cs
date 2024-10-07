using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterList_UI : MonoBehaviour
{
    public RectTransform myRect;
    public RectTransform onButton;
    public RectTransform closeButton;
    public RectTransform listUI;
    public RectTransform content;
    public CharacterListSlot_UI unitSlot;
    private Vector3 startPos;
    private float animDelay = 0.5f;
    [Header("UI_Info")]
    public bool isUse = false; //UI ÄÑÁú ½Ã true
    public List<GameObject> unitSlots = new List<GameObject>();
    protected bool isAnimPlaying = false;

    private void Awake()
    {
        myRect = GetComponent<RectTransform>();
        startPos = myRect.anchoredPosition;
        unitSlot = Resources.Load<CharacterListSlot_UI>("Prefabs/UI/CharacterList_Slot");
        Recycle();
    }

    protected void Recycle()
    {
        closeButton.gameObject.SetActive(false);
        listUI.gameObject.SetActive(false);

        isUse = false;
    }

    public void OpenList()
    {
        StartCoroutine(ListOpenAnimation());
    }
    
    public void CloseList()
    {
        StartCoroutine(ListCloseAnimation());
    }

    protected IEnumerator ListOpenAnimation()
    {
        if (!isAnimPlaying)
        {
            isAnimPlaying = true;
            isUse = true;

            closeButton.gameObject.SetActive(true);
            listUI.gameObject.SetActive(true);
            ListReSetting();

            LeanTween.moveY(myRect, startPos.y + 250f, animDelay).setEase(LeanTweenType.easeInOutQuart);
            yield return new WaitForSeconds(animDelay);


            isAnimPlaying = false;
        }

    }

    protected IEnumerator ListCloseAnimation()
    {
        if (!isAnimPlaying)
        {
            isAnimPlaying = true;
            isUse = false;
            FieldManager.instance.cameraUsable.trackingTarget = null;

            LeanTween.moveY(myRect, startPos.y, animDelay).setEase(LeanTweenType.easeInOutQuart);
            yield return new WaitForSeconds(animDelay);

            closeButton.gameObject.SetActive(false);
            listUI.gameObject.SetActive(false);

            isAnimPlaying = false;
        }
    }

    protected void ListReSetting()
    {
        ListClear();

        int heroCount = FieldManager.instance.heroList.Count;

        for(int i = 0; i < heroCount; i++)
        {
            unitSlot.character = FieldManager.instance.heroList[i];
            unitSlot.Recycle();

            GameObject slot = PoolManager.instance.Spawn(unitSlot.gameObject, Vector3.zero, Vector3.one, Quaternion.identity, true, content);
            unitSlots.Add(slot); 
        }
    }

    protected void ListClear()
    {
        foreach(GameObject slot in unitSlots)
        {
            Disapper(slot);
        }

        unitSlots.Clear();
    }

    protected void Disapper(GameObject gameObject)
    {
        PoolManager.instance.Release(gameObject);
    }
}
