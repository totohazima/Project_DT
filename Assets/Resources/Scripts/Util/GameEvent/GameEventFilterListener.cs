using UnityEngine;
using UnityEngine.Events;

namespace GameEvent
{
    public class GameEventFilterListener : MonoBehaviour
    {
        [Tooltip("InstanceID from GameObject")]
        public GameObject keyObject = null;

        [Tooltip("Event to register with.")]
        public GameEventFilter Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent Response;

        private void OnEnable()
        {
            if (keyObject == null)
                keyObject = gameObject;
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            Response.Invoke();
        }
    }
}