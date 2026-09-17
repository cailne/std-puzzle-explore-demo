using System;

namespace Lucielle
{
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
