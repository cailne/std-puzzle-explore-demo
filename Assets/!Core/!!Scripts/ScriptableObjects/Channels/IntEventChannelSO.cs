using System;
using UnityEngine;

namespace Lucielle
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Events/Int Event Channel")]
    public class IntEventChannelSO : BaseEventChannelSO<int>
    {
        public override void RegisterListener(Action<int> action)
        {
            onParameterEventRaised += action;
        }

        public override void RemoveListener(Action<int> action)
        {
            onParameterEventRaised -= action;
        }
    }
}
