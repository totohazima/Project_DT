using GDBA;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UICharacterCostume : MonoBehaviour
{
    [System.Serializable]
    public struct SCostumeInfo
    {
        public string key;
        public SpriteRenderer[] renderers;
    }
    public SCostumeInfo[] costumeInfos = null;

    [System.Serializable]
    public struct SWingCostumeInfo
    {
        public string key;
        public SpriteRenderer[] renderers;
    }
    public SWingCostumeInfo[] wingCostumeInfos = null;
    [System.Serializable]
    public struct SWeaponCostumeInfo
    {
        public string key;
        public SpriteRenderer[] renderers;
    }
    public SWeaponCostumeInfo[] weaponCostumeInfos = null;

}