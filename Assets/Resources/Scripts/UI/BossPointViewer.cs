using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class BossPointViewer : MonoBehaviour, ICustomUpdateMono
{
    public new Transform getTransform;
    public FieldActivity fieldActivity;

    public TextMeshPro bPointText;
    public SpriteRenderer bPointBar;
    private Transform bPointTransform;
    private Vector3 bPointScale = Vector3.one;


    void Awake()
    {
        if (bPointBar != null)
        {
            bPointTransform = bPointBar.transform;
            getTransform = gameObject.transform;
            bPointScale = bPointBar.transform.localScale;
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
        if (fieldActivity == null)
        {
            return;
        }

        float fillAmount = 0F;
        if (fieldActivity.bossPoint <= 0)
        {
            fillAmount = 0;
        }
        else
        {
            fillAmount = Mathf.Clamp01((float)(fieldActivity.bossPoint / 100f));
        }    

        bPointTransform.localScale = new Vector3(bPointScale.x * fillAmount, bPointScale.y, bPointScale.z);

        if (fillAmount > 1F)
        {
            fillAmount = 1F;
        }

        bPointText.text = fieldActivity.bossPoint + " / " + fieldActivity.maxBossPoint;
    }
}
