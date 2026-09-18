using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lucielle
{
	public class CombinationLock : MonoBehaviour
	{
		[Header("Modules")]
		[SerializeField] private List<int> targetCombination = new ();
		[SerializeField] private List<NumberDisplayLock> numberDisplayLocks = new();
		[SerializeField] private GameObject gameTransform;

		[Space(5f), Header("Channels")]
		[SerializeField] private IntEventChannelSO onLockNumberChangedEventChannelSO;

		private bool started = false;

		private void OnEnable()
		{
			onLockNumberChangedEventChannelSO?.RegisterListener(CheckCombinationLock);
		}

		private void OnDisable()
		{
			onLockNumberChangedEventChannelSO?.RemoveListener(CheckCombinationLock);
		}

		public void Initialize()
		{
		}

		public void StartCombinationLock(List<int> combinationAnswer = null)
		{
			if (started)
			{
				gameTransform.SetActive(true);
				return;
			}

			started = true;
			targetCombination.Clear();
			List<int> target = null;
			if (combinationAnswer == null)
			{
				target = new();
				for (int i = 0; i < numberDisplayLocks.Count; i++)
				{
					int rnd = Random.Range(0, 10);
					target.Add(rnd);
				}
			}
			else target = combinationAnswer;

			targetCombination = target;
			foreach (var t in numberDisplayLocks)
			{
				t.Initialize();
			}

			//this maybe not needed for trimming, especially we just for-loop the numberDisplayLock count
			// if (target.Count > numberDisplayLocks.Count)
			// 	target.RemoveRange(numberDisplayLocks.Count, target.Count - numberDisplayLocks.Count);
		}

		private void CheckCombinationLock(int overload)
		{
			//overload param not used
			bool complete = true;
			for (int i = 0; i < numberDisplayLocks.Count; i++)
			{
				if (numberDisplayLocks[i].CurrentNumber == targetCombination[i]) continue;
				complete = false;
				break;
			}

			if (complete)
			{
				started = false;
			}
		}
	}
}
