using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameEvent
{
    [CreateAssetMenu(menuName = "GameEvent/FilterEvent")]
    public class GameEventFilter : ScriptableObject
    {
        [System.Serializable]
        public struct SListener
        {
            public int nKey;    //InstanceID;
            //public List<GameEventFilterListener> listeners;
            public List<UnityEvent> listeners;
        }

        private readonly List<SListener> eventListeners = new List<SListener>();

        /*
         * *
         */

        public void Raise(GameObject _instance)
        {
            int myKey = _instance.GetInstanceID();
            Raise(myKey);
        }

        public void Raise(int _instanceID)
        {
            SListener data;
            //int Count = eventListeners.Count;
            // int listenerCount = 0;
            for (int i = 0; i < eventListeners.Count; i++)
            {
                data = eventListeners[i];
                if (data.nKey != _instanceID)
                    continue;

                /**/
                //listenerCount = data.listeners.Count;
                for (int j = 0; j < data.listeners.Count; j++)
                {
                    //data.listeners[j].OnEventRaised();
                    //if (listenerCount <= j)
                    //    Debug.Log("");
                    data.listeners[j].Invoke();
                }
            }
        }

        public void RegisterListener(GameObject keyObject, UnityEvent response)
        {
            SListener sListener = eventListeners.Find((a) => a.nKey == keyObject.GetInstanceID());
            if (sListener.Equals(default(SListener)) == true)
            {
                sListener = new SListener();
                sListener.nKey = keyObject.GetInstanceID();
                //sListener.listeners = new List<GameEventFilterListener>();
                sListener.listeners = new List<UnityEvent>();
                eventListeners.Add(sListener);
            }

            if (sListener.listeners.Contains(response) == false)
                sListener.listeners.Add(response);
        }
        public void RegisterListener(GameEventFilterListener listener)
        {
            SListener sListener = eventListeners.Find((a) => a.nKey == listener.keyObject.GetInstanceID());
            if (sListener.Equals(default(SListener)) == true)
            {
                sListener = new SListener();
                sListener.nKey = listener.keyObject.GetInstanceID();
                //sListener.listeners = new List<GameEventFilterListener>();
                sListener.listeners = new List<UnityEvent>();
                eventListeners.Add(sListener);
            }

            if (sListener.listeners.Contains(listener.Response) == false)
                sListener.listeners.Add(listener.Response);

        }

        public void UnregisterListener(GameObject keyObject)
        {
            SListener sListener = eventListeners.Find((a) => a.nKey == keyObject.GetInstanceID());
            if (sListener.Equals(default(SListener)) == true)
                return;

            //for(int i = 0; i < sListener.listeners.Count; i++)
            while (sListener.listeners.Count > 0)
            {
                sListener.listeners.RemoveAt(0);
            }
            eventListeners.Remove(sListener);
        }

        public void UnregisterListener(GameObject keyObject, UnityEvent response)
        {
            SListener sListener = eventListeners.Find((a) => a.nKey == keyObject.GetInstanceID());
            if (sListener.Equals(default(SListener)) == true)
                return;

            if (sListener.listeners.Contains(response) == true)
                sListener.listeners.Remove(response);

            if (sListener.listeners.Count <= 0)
                eventListeners.Remove(sListener);
        }
        public void UnregisterListener(GameEventFilterListener listener)
        {
            SListener sListener = eventListeners.Find((a) => a.nKey == listener.keyObject.GetInstanceID());
            if (sListener.Equals(default(SListener)) == true)
                return;

            if (sListener.listeners.Contains(listener.Response) == true)
                sListener.listeners.Remove(listener.Response);

            if (sListener.listeners.Count <= 0)
                eventListeners.Remove(sListener);
        }
    }
}