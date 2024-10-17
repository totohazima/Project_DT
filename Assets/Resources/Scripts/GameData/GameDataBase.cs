using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

namespace GDBA
{
    [CreateAssetMenu(menuName = "GameDataBase/GameDataBase")]
    public class GameDataBase : ScriptableObject
    {
        public static GameDataBase instance = null;
        public PlayerInfo playerInfo = null;
        public void InitInstance()
        {
            instance = this;
        }

        [Header("Character")]
        public CharacterInfoTable characterInfoTable;

        [Header("Item")]
        public GameMoneyInfoTable gameMoneyInfoTable;

        [Header("Equipment")]
        public EquipmentInfoTable weaponInfoTable;
        public EquipmentInfoTable helmetInfoTable;

        [Header("SpriteAtlas")]
        public SpriteAtlas weaponSpriteAtlas;
        public SpriteAtlas helmetSpriteAtlas;
        public SpriteAtlas gameMoneyAtlas;
    }


}
