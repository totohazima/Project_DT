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
        public float AttackSpeedPercent
        {
            get { return attackSpeedPercent; }
            set { attackSpeedPercent = value; }
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
        public float MoveSpeedPercent
        {
            get { return moveSpeedPercent; }
            set { moveSpeedPercent = value; }
        }

        public double VIewRange
        {
            get { return viewRange; }
            set { viewRange = value; }
        }

    }

    [System.Serializable]
    public class Desire_PlayStatus : Desire
    {
        public double Hunger
        {
            get { return hunger; }
            set { hunger = value; }
        }

        public double Sleep
        {
            get { return sleep; }
            set { sleep = value; }
        }

        public double Stress
        {
            get { return stress; }
            set { stress = value; }
        }
    }

    [System.Serializable]
    public class KDA_PlayStatus : KDA
    {
        public int Kill_Score
        {
            get { return kill_Score; }
            set { kill_Score = value; }
        }
        public int Death_Score
        {
            get { return death_Score; }
            set { death_Score = value; }
        }
        public int Assist_Score
        {
            get { return assist_Score; }
            set { assist_Score = value; }
        }
    }
}
