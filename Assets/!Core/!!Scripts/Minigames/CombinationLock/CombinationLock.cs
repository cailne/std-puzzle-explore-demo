using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lucielle
{
	public class CombinationLock : MonoBehaviour
	{
		[Header("Modules")]
		[SerializeField] private List<NumberDisplayLock> numberDisplayLocks = new();
		[SerializeField] private GameObject gameParent;
		[SerializeField] private GameObject falseIndicator;
		[SerializeField] private Transform historyContainer;
		[SerializeField] private HistoryCard historyCard;

		[Space(5f), Header("Properties")]
		[SerializeField, ReadOnly] private List<int> targetCombination = new ();

		[Space(5f), Header("Channels")]
		[SerializeField] private VoidEventChannelSO combinationLockStartEventChannelSO;
		[SerializeField] private BoolEventChannelSO lockAnimationDoneEventChannelSO;
		[SerializeField] private BoolEventChannelSO controlActivationEventChannelSO;

		private List<HistoryCard> pooledHistoryCards = new();
		private bool isPlayAllowed = false;

		private void Awake()
		{
			Initialize();
		}

		private void OnEnable()
		{
			combinationLockStartEventChannelSO?.RegisterListener(() => StartCombinationLock());
			lockAnimationDoneEventChannelSO?.RegisterListener(SetPlayState);
		}

		private void OnDisable()
		{
			combinationLockStartEventChannelSO?.RemoveListener(() => StartCombinationLock());
			lockAnimationDoneEventChannelSO?.RemoveListener(SetPlayState);
		}

		private void Initialize()
		{
			//warm up pool for history cards
			for (int i = 0; i < 10; i++)
			{
				HistoryCard card = Instantiate(historyCard, historyContainer);
				card.gameObject.SetActive(false);
				pooledHistoryCards.Add(card);
			}
            gameParent.SetActive(false);
		}

		[Button]
		public void StartCombinationLock(List<int> combinationAnswer = null)
		{
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
			gameParent.SetActive(true);

			controlActivationEventChannelSO?.RaiseEvent(false);
			lockAnimationDoneEventChannelSO?.RaiseEvent(true);
			//this maybe not needed for trimming, especially we just for-loop the numberDisplayLock count
			// if (target.Count > numberDisplayLocks.Count)
			// 	target.RemoveRange(numberDisplayLocks.Count, target.Count - numberDisplayLocks.Count);
		}

		public void CheckCombinationLock()
		{
			if (!isPlayAllowed) return;

			bool complete = true;
			for (int i = 0; i < numberDisplayLocks.Count; i++)
			{
				if (numberDisplayLocks[i].CurrentNumber == targetCombination[i]) continue;
				complete = false;
				break;
			}

			if (!complete)
			{
				falseIndicator.gameObject.SetActive(false);
				falseIndicator.gameObject.SetActive(true);
				CreateHistory();
				return;
			}

			controlActivationEventChannelSO?.RaiseEvent(true);
			gameParent.SetActive(false);
			ResetHistoryCard();
		}

		private void CreateHistory()
		{
			List<COMBINATION_STATUS> status = EvaluateGuess(GetGuess(), targetCombination);
			HistoryCard card = InstantiateHistoryCard();
			card.Initialize(status);
			card.gameObject.SetActive(true);
			card.transform.SetAsFirstSibling();
		}

		private HistoryCard InstantiateHistoryCard()
		{
			for (int i = 0; i < pooledHistoryCards.Count; i++)
			{
				if (!pooledHistoryCards[i].gameObject.activeInHierarchy)
					return pooledHistoryCards[i];
			}

			HistoryCard card = Instantiate(historyCard, historyContainer);
			pooledHistoryCards.Add(card);
			return card;
		}

		private void ResetHistoryCard()
		{
			for (int i = 0; i < pooledHistoryCards.Count; i++)
				pooledHistoryCards[i].gameObject.SetActive(false);
		}

		private List<int> GetGuess()
		{
			List<int> results = new();
			for (int i = 0; i < numberDisplayLocks.Count; i++)
			{
				results.Add(numberDisplayLocks[i].CurrentNumber);
			}
			return results;
		}

		private List<COMBINATION_STATUS> EvaluateGuess(List<int> guess, List<int> target)
		{
			int length = Mathf.Min(guess.Count, target.Count);
			List<COMBINATION_STATUS> results = new(length);

			bool[] targetMatched = new bool[length];
			bool[] guessMatched = new bool[length];

			for (int i = 0; i < length; i++)
			{
				if (guess[i] != target[i]) continue;
				results[i] = COMBINATION_STATUS.RIGHT;
				targetMatched[i] = true;
				guessMatched[i] = true;
			}

			for (int i = 0; i < length; i++)
			{
				if (guessMatched[i]) continue;

				bool foundYellow = false;
				for (int j = 0; j < length; j++)
				{
					if (!targetMatched[j] && guess[i] != target[j]) continue;
					results[i] = COMBINATION_STATUS.CLOSE;
					targetMatched[j] = true;
					foundYellow = true;
					break;
				}

				if (!foundYellow) results[i] = COMBINATION_STATUS.NOTHING;
			}
			return results;
		}

		private void SetPlayState(bool allowed)
		{
			isPlayAllowed = allowed;
		}
	}
}
