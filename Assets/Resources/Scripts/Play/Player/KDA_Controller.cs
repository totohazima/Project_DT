using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDA_Controller : MonoBehaviour
{ 
    // 공격자 정보 저장용 클래스
    class AttackerInfo
    {
        public Character attacker; // 공격자 객체
        public Coroutine countdownCoroutine; // 타이머용 코루틴
    }

    // 공격자 리스트와 타이머 관리
    private List<AttackerInfo> attackerList = new List<AttackerInfo>();
    private Dictionary<Character, AttackerInfo> attackerTimers = new Dictionary<Character, AttackerInfo>();

    // 공격자가 들어올 때 호출하는 함수
    public void OnAttacked(Character attacker)
    {
        if (attackerTimers.ContainsKey(attacker))
        {
            // 이미 리스트에 있으면 타이머 초기화
            ResetAttackerTimer(attacker);
        }
        else
        {
            // 새로운 공격자 정보를 추가
            AddAttacker(attacker);
        }
    }

    // 새로운 공격자 추가 함수
    protected void AddAttacker(Character attacker)
    {
        // 공격자 정보를 리스트에 추가
        AttackerInfo newAttackerInfo = new AttackerInfo
        {
            attacker = attacker,
            countdownCoroutine = StartCoroutine(RemoveAttackerAfterTime(attacker, 10f)) // 10초 타이머 시작
        };

        attackerList.Add(newAttackerInfo);
        attackerTimers.Add(attacker, newAttackerInfo);
    }

    // 타이머 초기화 함수
    protected void ResetAttackerTimer(Character attacker)
    {
        if (attackerTimers.ContainsKey(attacker))
        {
            // 기존 타이머 중지
            StopCoroutine(attackerTimers[attacker].countdownCoroutine);
            // 새 타이머 시작
            attackerTimers[attacker].countdownCoroutine = StartCoroutine(RemoveAttackerAfterTime(attacker, 10f));
        }
    }

    // 공격자를 10초 후 리스트에서 제거하는 함수
    protected IEnumerator RemoveAttackerAfterTime(Character attacker, float time)
    {
        yield return new WaitForSeconds(time);

        // 타이머 종료 후 공격자를 리스트와 Dictionary에서 제거
        if (attackerTimers.ContainsKey(attacker))
        {
            attackerList.Remove(attackerTimers[attacker]);
            attackerTimers.Remove(attacker);
        }
    }

    public void KDA_Calculator(Character killAttacker, Character myCharacter)
    {
        foreach (AttackerInfo attackerInfo in attackerList)
        {
            if (attackerInfo.attacker == killAttacker)
            {
                attackerInfo.attacker.playStatus_KDA.kill_Score++;
            }
            else
            {
                attackerInfo.attacker.playStatus_KDA.assist_Score++;
            }
        }

        myCharacter.playStatus_KDA.death_Score++;
    }

}

