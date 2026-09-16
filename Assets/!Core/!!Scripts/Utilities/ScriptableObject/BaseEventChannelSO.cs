using System;

namespace Lucielle
{
    public class BaseEventChannelSO : DescriptionSO
    {
        protected Action onEventRaised;

        public void RaiseEvent()
        {
            onEventRaised?.Invoke();
        }

        public void RegisterListener(Action action)
        {
            onEventRaised += action;
        }

        public void RemoveListener(Action action)
        {
            onEventRaised -= action;
        }
    }

    public abstract class BaseEventChannelSO<T> : BaseEventChannelSO
    {
        protected Action<T> onParameterEventRaised;

        public void RaiseEvent(T value)
        {
            onParameterEventRaised?.Invoke(value);
            base.RaiseEvent();
        }

        public abstract void RegisterListener(Action<T> action);

        public abstract void RemoveListener(Action<T> action);
    }
}
