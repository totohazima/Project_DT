using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;

public class Building : FieldObject, IPointerDownHandler, IPointerUpHandler, IDragHandler 
{
    [HideInInspector] public Transform getTransform;
    public bool isDragged;
    public bool isClicked;
    public bool isLongClicked;
    private float keepClickTimer = 0f;
    private float longClickTime = 0.7f;

    void Awake()
    {
        getTransform = transform;
    }
    public virtual void Update()
    {
        ClickCheck(); 
    }

    private void ClickCheck()
    {
        if (isClicked)
        {
            keepClickTimer += Time.deltaTime;
            if (keepClickTimer >= longClickTime)
            {
                isLongClicked = true;
            }
        }
        else
        {
            keepClickTimer = 0f;
        }

        if (isLongClicked)
        {
            FieldActivity.instance.cameraDrag.isDontMove = true;
        }
        else
        {
            FieldActivity.instance.cameraDrag.isDontMove = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isClicked = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isClicked = false;
        isLongClicked = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!isLongClicked)
        {
            return;
        }

        Vector3 dragPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        dragPosition = new Vector3(dragPosition.x, dragPosition.y, getTransform.position.z);
        getTransform.position = dragPosition;
    }

   
}
