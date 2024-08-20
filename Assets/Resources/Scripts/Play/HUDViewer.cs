using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// 캐릭터 오브젝트의 UI(체력바)를 표기하는 스크립트
/// </summary>
public class HUDViewer : MonoBehaviour, ICustomUpdateMono
{
    public Character character;
    public TextMeshPro hpTxt;
    public SpriteRenderer hpBar;
    private Transform hpBarTransform;
    private Vector3 hpScale = Vector3.one;

    private void Awake()
    {
        if(hpBar != null)
        {
            hpBarTransform = hpBar.transform;
            hpScale = hpBar.transform.localScale;
        }
    }
    private void OnEnable()
    {
        CustomUpdateManager.customUpdateMonos.Add(this);
    }

    private void OnDisable()
    {
        CustomUpdateManager.customUpdateMonos.Remove(this);
    }

    public void CustomUpdate()
    {
        if (character == null && character.playStatus == null)
        {
            return;
        }

        if(hpTxt != null)
        {
            hpTxt.text = "<sprite=3>" + character.playStatus.CurHealth;
        }

        float fillAmount = 0F;
        if (character.playStatus.CurHealth <= 0)
        {
            fillAmount = 0;
        }
        else
        {
            fillAmount = Mathf.Clamp01((float)(character.playStatus.CurHealth / character.playStatus.MaxHealth));
        }
        hpBarTransform.localScale = new Vector3(hpScale.x * fillAmount, hpScale.y, hpScale.z);

        //if (character.viewObject.eulerAngles.x > 0)
        //{
        //    transform.eulerAngles = new Vector3(-50, 180, 0);
        //}
        //else
        //{
        //    transform.eulerAngles = new Vector3(50, 180, 0);
        //}
    }

}
