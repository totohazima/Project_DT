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
        ATTACK_SPEED = 2,
        MAX_HEALTH = 3,
        MOVE_SPEED = 4,
        VIEW_RANGE = 5,
    }

    [System.Serializable]
    public class Status
    {
        public double attackPower = 0;
        public double attackRange = 0;
        public double attackSpeed = 0;
        public double maxHealth = 0;
        public double moveSpeed = 0;
        public double viewRange = 0;

        public void Reset()
        {
            attackPower = 0;
            attackRange = 0;
            attackSpeed = 0;
            maxHealth = 0;
            moveSpeed = 0;
            viewRange = 0;
        }
    }
}
