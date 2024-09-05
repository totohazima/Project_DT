using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatusHelper;

public class StatusInfo : ScriptableObject
{
    [System.Serializable]
    public class CharacterStatus : Status
    {
        public double newAttackPower = 0;
        public double newAttackRange = 0;
        public double newAttackSpeed = 0;
        public double newMaxHealth = 0;
        public double newMoveSpeed = 0;
        public double newViewRange = 0;
    }

    /// <summary>
    /// Ω∫≈» ∞ËªÍ
    /// </summary>
    public void CalculateStatus()
    {

    }
}
