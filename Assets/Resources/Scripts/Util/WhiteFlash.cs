using System.Collections;
using UnityEngine;

public class WhiteFlash : MonoBehaviour
{

    BetterList<SpriteRenderer> spriteRenderers = new BetterList<SpriteRenderer>();
    [SerializeField]
    private float flashDurationTime = 1.0f;
    [SerializeField]
    private LeanTweenType tweenType = LeanTweenType.easeOutExpo;

    class TintColorScedule
    {
        public object callerObject;
        public string startOrEndStr;
        public Color color;

        public TintColorScedule(object callerObject, string startOrEndStr, Color color) //2019-12-12 상은 : 빙결과 같이 캐릭터에 색깔 입힐때 색깔이 중첩되기 위해 추가
        {
            this.startOrEndStr = startOrEndStr;
            this.color = color;
            this.callerObject = callerObject;
        }
    }

    BetterList<TintColorScedule> tintColorSceduleCallStack = new BetterList<TintColorScedule>(); //2019-12-12 상은 : 빙결과 같이 캐릭터에 색깔 입힐때 색깔이 중첩되기 위해 추가

    private void Awake()
    {
        SpriteRenderer[] renderes = GetComponentsInChildren<SpriteRenderer>(true);
        for (int i = 0; i < renderes.Length; i++)
        {
            spriteRenderers.Add(renderes[i]);
        }
    }

    private void OnEnable()
    {
        if (spriteRenderers.size > 0)
        {
            SpriteRenderer spriteRenderer;
            int spriteRenderersLength = spriteRenderers.size;
            for (int i = 0; i < spriteRenderersLength; i++)
            {
                spriteRenderer = spriteRenderers.buffer[i];
                if (spriteRenderer != null)
                    spriteRenderer.material.SetFloat("_FlashAmount", 0);
            }
             
        }
    }

    public void SetSceduleForTintColor(object callerObject, string startOrEndStr, Color color) //2019-12-12 상은 : 빙결과 같이 캐릭터에 색깔 입힐때 색깔이 중첩되기 위해 추가
    {
        bool removed = false;
        if (startOrEndStr.Equals("end"))
        {
            for (int i = 0; i < tintColorSceduleCallStack.size; i++)
            {
                if (tintColorSceduleCallStack.buffer[i].callerObject.Equals(callerObject))
                {
                    if (tintColorSceduleCallStack.size == 1)
                    {
                        SetTintColor(color);
                    }
                    tintColorSceduleCallStack.RemoveAt(i);
                    removed = true;
                }
            }
        }
        if (removed)
        {
            for (int i = 0; i < tintColorSceduleCallStack.size; i++)
            {
                if (tintColorSceduleCallStack.buffer[i].startOrEndStr.Equals("start"))
                {
                    SetTintColor(tintColorSceduleCallStack.buffer[i].color);
                    return;
                }
            }
        }

        TintColorScedule tintColorScedule = new TintColorScedule(callerObject, startOrEndStr, color);
        tintColorSceduleCallStack.Add(tintColorScedule);
        SetTintColor(color);
    }

    void SetTintColor(Color color)
    {
        SpriteRenderer spriteRenderer;
        for (int i = 0; i < spriteRenderers.size; i++)
        {
            spriteRenderer = spriteRenderers.buffer[i];
            spriteRenderer.material.SetColor("_Color", color);
        }
    }

    public void PlayFlash()
    {
        StepByStepPlay(flashDurationTime);
    }

    public void PlayFlash(float customDurationTime)
    {
        StepByStepPlay(customDurationTime);
    }

    void DirectPlay()
    {
        SpriteRenderer spriteRenderer;
        for (int i = 0; i < spriteRenderers.size; i++)
        {
            spriteRenderer = spriteRenderers.buffer[i];
            spriteRenderer.material.SetFloat("_FlashAmount", 1);
        }
    }

    IEnumerator coDirectPlay()
    {
        SpriteRenderer spriteRenderer;

        for (int i = 0; i < spriteRenderers.size; i++)
        {
            spriteRenderer = spriteRenderers.buffer[i];
            spriteRenderer.material.SetFloat("_FlashAmount", 1);
        }
        yield return new WaitForSeconds(flashDurationTime * 0.2f);
        for (int i = 0; i < spriteRenderers.size; i++)
        {
            spriteRenderer = spriteRenderers.buffer[i];
            spriteRenderer.material.SetFloat("_FlashAmount", 0);
        }
    }

    void StepByStepPlay(float durationTime)
    {        
        float halfTime = durationTime * 0.2f;
        SpriteRenderer spriteRenderer;

        int spriteRenderersLength = spriteRenderers.size;
        LeanTween.value(0.0f, 1.0f, halfTime).setOnUpdate((float val) =>
        {
            for (int i = 0; i < spriteRenderersLength; i++)
            {

                spriteRenderer = spriteRenderers.buffer[i];
                if (spriteRenderer != null)
                    spriteRenderer.material.SetFloat("_FlashAmount", val);
            }
        }).setEase(tweenType).setOnComplete(() =>
        {
            LeanTween.value(1.0f, 0.0f, halfTime).setOnUpdate((float val) =>
            {
                for (int i = 0; i < spriteRenderersLength; i++)
                {

                    spriteRenderer = spriteRenderers.buffer[i];
                    if (spriteRenderer != null)
                        spriteRenderer.material.SetFloat("_FlashAmount", val);
                }
            }).setEase(tweenType);
        });
    }
}
