using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucielle
{
	public class LockInteractables : MonoBehaviour
	{
		[SerializeField] private BoolEventChannelSO onLockInteractableEventChannelSO;

		public void ClickUp()
		{
			onLockInteractableEventChannelSO?.RaiseEvent(true);
		}

		public void ClickDown()
		{
			onLockInteractableEventChannelSO?.RaiseEvent(false);
		}
	}
}
