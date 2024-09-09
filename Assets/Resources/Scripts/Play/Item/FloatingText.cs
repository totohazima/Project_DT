using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GameSystem;
/// <summary>
/// 인 게임 상에서 플레이중에 띄워지는 텍스트들 (UI 아님)
/// </summary>
public class FloatingText : MonoBehaviour
{
    public new Transform transform;
    public TextMeshPro text;
    void Awake()
    {
        transform = GetComponent<Transform>();  
    }

    public virtual void OnEnable()
    {
        StartCoroutine(Text_Animation());
    }
    
    public virtual IEnumerator Text_Animation()
    {
        yield return 0;
    }

    public virtual void Disappeer()
    {
        PoolManager.instance.Release(gameObject);
    }
}
