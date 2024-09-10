using GameSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.Events;

namespace GameEvent
{
    public class EventCallAnimation : MonoBehaviour
    {
        public Character character;
        public GameObject callPrefab;
        public GameObject keyObject = null;
        public UnityEvent customEvent;
        public Transform firePosition;
        public void CallFromFilterAnimtion(GameEventFilter gameEvent)
        {
            //if (keyObject == null)
            //    keyObject = gameObject;
            //int myKey = keyObject.GetInstanceID();

            //if (customEvent != null)
            //{
            //    customEvent.Invoke();
            //}
            //gameEvent.Raise(myKey);
            AttackObject_Create();
        }
        public void SoundFxPlay(GameEventFilter gameEvent)
        {
            
        }

        private void AttackObject_Create()
        {
            //GameObject Prefab = Resources.Load<GameObject>("Prefabs/AttackObject/Hero_Attack_Devil");

            GameObject attackPrefab = PoolManager.instance.Spawn(callPrefab, firePosition.position, Vector3.one, Quaternion.identity, true, firePosition);
            attackPrefab.transform.position = firePosition.position;

            AttackObject attackObject = attackPrefab.GetComponent<AttackObject>();
            Character target = null;
            if (character.targetUnit != null)
            {
                target = character.targetUnit.GetComponentInParent<Character>();
            }

            attackObject.Recycle(character, target);
        }
    }
}
