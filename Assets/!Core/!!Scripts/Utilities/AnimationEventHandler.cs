using UnityEngine;
using UnityEngine.Events;

namespace Lucielle
{
    public class AnimationEventHandler : MonoBehaviour
    {
        [System.Serializable]
        public class AnimationEvent
        {
            public string eventName;
            public UnityEvent invokeEvent;

            public void Invoke()
            {
                invokeEvent?.Invoke();
            }
        }

        [SerializeField] private AnimationEvent[] animationEvents;

        public void TriggerEvent(string eventName)
        {
            foreach (var animEvent in animationEvents)
            {
                if (animEvent.eventName == eventName)
                {
                    animEvent.Invoke();
                    return;
                }
            }
            DebugManager.LogWarning($"Animation event '{eventName}' not found on GameObject '{gameObject.name}'.");
        }
    }
}
