using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Pathfinding.GridGraph;

public class CharacterListSlot_UI : MonoBehaviour
{
    public HeroCharacter character;
    public Image characterIcon;
    public Text characterName;

    public void Recycle()
    {
        if(character == null)
        {
            return;
        }

        IconSetting();
        characterName.text = character.characterName;
    }
    private void IconSetting()
    {
        string iconCode = "Texture/Thumbnail_Rankingimg/" + character.characterCostume.dressCostume_Code + "_Ranking";
        Sprite costumeSprite = Resources.Load<Sprite>(iconCode);
        characterIcon.sprite = costumeSprite;
    }
    public void AutoTrack()
    {
        FieldManager.instance.cameraUsable.trackingTarget = character;
    }
}
