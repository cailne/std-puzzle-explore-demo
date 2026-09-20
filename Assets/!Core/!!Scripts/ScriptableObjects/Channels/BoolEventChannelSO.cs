using System;
using UnityEngine;

namespace Lucielle
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Events/Bool Event Channel")]
	public class BoolEventChannelSO : BaseEventChannelSO<bool>
	{
		public override void RegisterListener(Action<bool> action)
		{
			onParameterEventRaised += action;
		}
		public override void RemoveListener(Action<bool> action)
		{
			onParameterEventRaised -= action;
		}
	}
}
