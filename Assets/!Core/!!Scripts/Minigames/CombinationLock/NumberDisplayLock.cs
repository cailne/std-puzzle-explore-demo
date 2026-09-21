using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Lucielle
{
    public class NumberDisplayLock : MonoBehaviour
    {
        [Header("Modules")]
        [SerializeField] private int defaultDigit = 4;
        [SerializeField] private float numberSpacing = 278f;
        [SerializeField] private float baseYContainerPosition = -35f;
        [SerializeField] private RectTransform numberContainer;

        [Space(5f), Header("Channels")]
        [SerializeField] private BoolEventChannelSO onLockInteractableEventChannelSO;
        [SerializeField] private IntEventChannelSO onLockNumberChangedEventChannelSO;
		[SerializeField] private BoolEventChannelSO lockAnimationDoneEventChannelSO;

        private int currentNumber = 0;

        public int CurrentNumber => currentNumber;

        private void Awake()
        {
            baseYContainerPosition = numberContainer.anchoredPosition.y;
        }

        private void OnEnable()
        {
            onLockInteractableEventChannelSO?.RegisterListener(TurnLock);
        }

        private void OnDisable()
        {
            onLockInteractableEventChannelSO?.RemoveListener(TurnLock);
        }

        public void Initialize(int number = 4)
        {
            currentNumber = number;
            SetColumnDigit();
        }

        private void TurnLock(bool isUp)
        {
            if (numberContainer == null) return;

            switch (isUp)
            {
                case true when currentNumber == 0:
                case false when currentNumber == 9:
                    return;
                case true:
                    currentNumber--;
                    break;
                default:
                    currentNumber++;
                    break;
            }

            SetColumnDigitWithAnimation();
        }

        private float GetYForDigit(int digit)
        {
            digit = Mathf.Clamp(digit, 0, 9);
            return baseYContainerPosition + ((digit - defaultDigit) * numberSpacing);
        }

        private void SetColumnDigit()
        {
            Vector2 anchoredPos = numberContainer.anchoredPosition;
            anchoredPos.y = GetYForDigit(currentNumber);
            numberContainer.anchoredPosition = anchoredPos;
            onLockNumberChangedEventChannelSO?.RaiseEvent(currentNumber);
        }

        private void SetColumnDigitWithAnimation()
        {
            lockAnimationDoneEventChannelSO?.RaiseEvent(false);

            Vector2 anchoredPos = numberContainer.anchoredPosition;
            anchoredPos.y = GetYForDigit(currentNumber);

            numberContainer.DOAnchorPos(anchoredPos, 0.25f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                onLockNumberChangedEventChannelSO?.RaiseEvent(currentNumber);
                lockAnimationDoneEventChannelSO?.RaiseEvent(true);
            });
        }
    }
}
