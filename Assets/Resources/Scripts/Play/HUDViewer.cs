using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// 캐릭터 오브젝트의 UI(체력바)를 표기하는 스크립트
/// </summary>
public class HUDViewer : MonoBehaviour, ICustomUpdateMono
{
    public new Transform getTransform;
    public Character character;

    public SpriteRenderer hpBar;
    private Transform hpBarTransform;
    private Vector3 hpScale = Vector3.one;

    public SpriteRenderer shieldBar = null;
    Vector3 shieldScale = Vector3.one;

    void Awake()
    {
        if(hpBar != null)
        {
            hpBarTransform = hpBar.transform;
            getTransform = gameObject.transform;
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
        if (character == null || character.playStatus == null)
            return;

        float fillAmount = 0F;
        if (character.playStatus.curHealth <= 0)
            fillAmount = 0;
        else
            fillAmount = Mathf.Clamp01((float)(character.playStatus.curHealth / character.playStatus.maxHealth));//Mathf.Clamp01(Mathf.InverseLerp(0, character.playStatus.maxHealthPoint, character.playStatus.healthPoint));
        hpBarTransform.localScale = new Vector3(hpScale.x * fillAmount, hpScale.y, hpScale.z);

        //if (goShield == null)
        //    return;
        // if (character.isHaveShield == false)
        //{
        //    goShield.SetActive(false);
        //    return;
        //}
        //goShield.SetActive(true);
        // fillAmount = (float)BigIntegerHelper.GetPercent(character.playStatus.ShieldPoint, character.playStatus.MaxShieldPoint);//Mathf.Clamp01(Mathf.InverseLerp(0, character.playStatus.maxHealthPoint, character.playStatus.healthPoint));
        if (fillAmount > 1F)
            fillAmount = 1F;
        // shieldBar.transform.localScale = new Vector3(shieldScale.x * fillAmount, shieldScale.y, shieldScale.z);
        //transform.localScale = character.viewObject.localScale;
        if (character.viewObject.eulerAngles.x > 0)
        {
            transform.eulerAngles = new Vector3(-50, 180, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(50, 180, 0);
        }
    }

}
