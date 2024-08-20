using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatusInfo;

namespace StatusHelper
{
    [System.Serializable]
    public class PlayStatus : Status
    {
        public double curHealth = 0;
        public double AttackPower
        {
            get { return attackPower; }
            set { attackPower = value; }
        }
        public double AttackRange
        {
            get { return attackRange; }
            set { attackRange = value; }
        }
        public double AttackSpeed
        {
            get { return attackSpeed; }
            set { attackSpeed = value; }
        }
        public double MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        }
        public double CurHealth
        {
            get { return curHealth; }
            set { curHealth = value; }
        }
        public double MoveSpeed
        {
            get { return moveSpeed; }
            set { moveSpeed = value; }
        }

        public double VIewRange
        {
            get { return viewRange; }
            set { viewRange = value; }
        }

    }
}
