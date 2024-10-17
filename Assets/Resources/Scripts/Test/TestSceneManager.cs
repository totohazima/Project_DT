using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class TestSceneManager : MonoBehaviour
{
    public SpriteAtlas spriteGroup;
    public SpriteRenderer spriteRenderer;
    private void Start()
    {
        spriteRenderer.sprite = spriteGroup.GetSprite("accessorySlotOpenTicekt_icon");
    }


}
