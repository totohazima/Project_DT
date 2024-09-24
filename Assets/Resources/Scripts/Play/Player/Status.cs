using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StatusHelper
{
    public enum ERISE_TYPE : int
    {
        PLUS = 0,   //합연산
        MULTIPLY = 1,   //곱연산
    }

    public enum ESTATUS : int
    {
        ATTACK_POWER = 0,
        ATTACK_RANGE = 1,
        ATTACK_SPEED_PERCENT = 2,
        MAX_HEALTH = 3,
        MOVE_SPEED_PERCENT = 4,
        VIEW_RANGE = 5,
    }

    public enum DESIRE_STATUS : int
    {
        HUNGER = 0,
        SLEEP = 1,
        STRESS = 2,
    }

    [System.Serializable]
    public class Status
    {
        public double attackPower = 0;
        public double attackRange = 0;
        public float attackSpeedPercent = 0;
        public double maxHealth = 0;
        public float moveSpeedPercent = 0;
        public double viewRange = 0;

        public void Reset()
        {
            attackPower = 0;
            attackRange = 0;
            attackSpeedPercent = 0;
            maxHealth = 0;
            moveSpeedPercent = 0;
            viewRange = 0;
        }
    }

    [System.Serializable]
    public class Desire
    {
        [Range(0f, 100f)] public double hunger = 100;
        [Range(0f, 100f)] public double sleep = 100;
        [Range(0f, 100f)] public double stress = 100;

        public void Reset()
        {
            hunger = 100;
            sleep = 100;
            stress = 100;
        }
    }

    [System.Serializable]
    public class KDA
    {
        public int kill_Score = 0;
        public int death_Score = 0;
        public int assist_Score = 0;

        public void Reset()
        {
            kill_Score = 0;
            death_Score = 0;
            assist_Score = 0;
        }
    }
}
