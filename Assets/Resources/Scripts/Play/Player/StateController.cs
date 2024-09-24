using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateController : MonoBehaviour, ICustomUpdateMono
{
    public Character character;
    public Animator animator;

    #region MoveSpeedParamaters
    const float MOVE_ANIMATION_SPEED_MIN = 0.1f;
    const float MOVE_ANIMATION_SPEED_MAX = 99.0f;
    [SerializeField]
    float defaultMoveSpeed = 1.0f;
    public float moveSpeed = 1.0f;
    public void SetMoveSpeedPercent(float percent)
    {
        moveSpeed = Mathf.Clamp(defaultMoveSpeed * percent, MOVE_ANIMATION_SPEED_MIN, MOVE_ANIMATION_SPEED_MAX);
    }
    #endregion
    #region AttackSpeedParamaters
    const float ATTACK_ANIMATION_SPEED_MIN = 0.1f;
    const float ATTACK_ANIMATION_SPEED_MAX = 99.0f;
    [SerializeField]
    float defaultAttackSpeed = 1.0f;
    public float DefaultAttackSpeed { get { return defaultAttackSpeed; } }
    public float attackSpeed = 1.0f;
    public void SetAttackSpeedPercent(float percent)
    {
        attackSpeed = Mathf.Clamp(defaultAttackSpeed * percent, ATTACK_ANIMATION_SPEED_MIN, ATTACK_ANIMATION_SPEED_MAX);
    }
    #endregion

    private void OnEnable()
    {
        CustomUpdateManager.customUpdateMonos.Add(this);    
    }


    private void OnDisable()
    {
        CustomUpdateManager.customUpdateMonos.Remove(this);    
    }

    public void CustomUpdate()
    {
        StatePlaySpeed();    
    }

    protected void StatePlaySpeed()
    {
        if(character.isReadyToAttack)
        {
            animator.speed = attackSpeed;
        }
        else
        {
            animator.speed = 1f;
        }
    }
}
