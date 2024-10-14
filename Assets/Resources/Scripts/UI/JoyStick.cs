using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, ICustomUpdateMono
{
    /// <summary>
    /// 기존 조이스틱 코드를 Canvas space-Camera 모드에서도 사용할 수 있게 수정
    /// </summary>
    public static JoyStick instance;

    private Vector3 DeathArea;
    [SerializeField] private RectTransform CenterReference;
    public GameObject joyStick;
    public GameObject stick;
    public bool isMove;
    //public List<PlayerCharacter> characterList = new List<PlayerCharacter>();
    [SerializeField] private float radius = 1;
    [SerializeField] private RectTransform stickRect;
    private RectTransform joyTrans;
    private RectTransform stickTrans;
    

    void Awake()
    {
        instance = this;
        joyTrans = joyStick.GetComponent<RectTransform>();
        stickTrans = stick.GetComponent<RectTransform>();
        //joyStick.SetActive(false);
    }

    void OnEnable()
    {
        CustomUpdateManager.customUpdateMonos.Add(this);
    }

    void OnDisable()
    {
        CustomUpdateManager.customUpdateMonos.Remove(this);
        stickTrans.anchoredPosition = CenterReference.anchoredPosition;
        joyStick.SetActive(false);
        isMove = false;
    }

    public void CustomUpdate()
    {

        //stickTrans.position = joyTrans.position;
        //joyStick.SetActive(false);
        //isMove = false;

        DeathArea = CenterReference.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //joyStick.SetActive(true);
        //joyTrans.position = eventData.pressPosition;
        //stickTrans.position = joyTrans.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Canvas가 ScreenSpace-Camera 모드일 경우 좌표 변환을 해야 함
        Vector3 dragPosition = UnityEngine.Camera.main.ScreenToWorldPoint(eventData.position);
        dragPosition = new Vector3(dragPosition.x, dragPosition.y, stickTrans.position.z);
        

        // 스틱이 원형 반경을 벗어나지 않도록 제한
        Vector3 centerPosition = joyTrans.position;
        centerPosition = new Vector3(centerPosition.x, centerPosition.y, joyTrans.position.z);

        float distance = Vector3.Distance(dragPosition, centerPosition);
        if (distance > radius)
        {
            Vector3 fromCenterToDrag = dragPosition - centerPosition;
            fromCenterToDrag = fromCenterToDrag.normalized * radius;
            Vector3 vec = centerPosition + fromCenterToDrag;

            Vector3 fixPos = new Vector3(vec.x, vec.y, stickTrans.position.z);
            stickTrans.position = fixPos;
        }
        else
        {
            stickTrans.position = dragPosition;
        }

        isMove = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        stickTrans.position = CenterReference.position;
        //joyStick.SetActive(false);
        isMove = false;
    }

    public float Horizontal
    {
        get
        {
            return Mathf.Clamp((stickRect.position.x - DeathArea.x) / radius, -1f, 1f);
        }
    }

    public float Vertical
    {
        get
        {
            return Mathf.Clamp((stickRect.position.y - DeathArea.y) / radius, -1f, 1f);
        }
    }
}

