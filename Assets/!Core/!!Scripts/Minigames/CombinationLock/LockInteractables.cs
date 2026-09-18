using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucielle
{
	public class LockInteractables : MonoBehaviour
	{
		[Header("Modules")]
		[SerializeField] private GameObject upButton;
		[SerializeField] private GameObject downButton;
		[Space(5f), Header("Channels")]
		[SerializeField] private BoolEventChannelSO onLockInteractableEventChannelSO;
		[SerializeField] private IntEventChannelSO onLockNumberChangedEventChannelSO;

		private void OnEnable()
		{
			onLockNumberChangedEventChannelSO?.RegisterListener(OnLockNumberChanged);
		}

		private void OnDisable()
		{
			onLockNumberChangedEventChannelSO?.RemoveListener(OnLockNumberChanged);
		}

		public void ClickUp()
		{
			onLockInteractableEventChannelSO?.RaiseEvent(true);
		}

		public void ClickDown()
		{
			onLockInteractableEventChannelSO?.RaiseEvent(false);
		}

		//specific only for 0 to 9
		private void OnLockNumberChanged(int number)
		{
			switch (number)
			{
				case 0:
					upButton.SetActive(false);
					downButton.SetActive(true);
					break;
				case 9:
					upButton.SetActive(true);
					downButton.SetActive(false);
					break;
				default:
					upButton.SetActive(true);
					downButton.SetActive(true);
					break;
			}
		}
	}
}
