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


        [Header("Equipment")]
        public EquipmentInfoTable weaponInfoTable = new EquipmentInfoTable();
        public EquipmentInfoTable helmetInfoTable = new EquipmentInfoTable();

        [Header("SpriteAtlas")]
        public SpriteAtlas weaponSpriteAtlas = new SpriteAtlas();
        public SpriteAtlas helmetSpriteAtlas = new SpriteAtlas();
    }


}
