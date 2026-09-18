using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucielle
{
    public class NumberDisplayLock : MonoBehaviour
    {
        [Header("Modules")]
        [SerializeField] private int defaultDigit = 4;
        [SerializeField] private float numberSpacing = 278f;
        [SerializeField] private RectTransform numberContainer;

        [Space(5f), Header("Channels")]
        [SerializeField] private BoolEventChannelSO onLockInteractableEventChannelSO;
        [SerializeField] private IntEventChannelSO onLockNumberChangedEventChannelSO;

        private int currentNumber = 0;
        private float baseYContainerPosition = 0;

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

        public void Initialize(int number = 5)
        {
            currentNumber = number;
            SetColumnDigit();
        }

        private void TurnLock(bool isUp)
        {
            if (numberContainer == null) return;

            if (isUp && currentNumber == 0) return;
            if (!isUp && currentNumber == 9) return;

            if (isUp) currentNumber--;
            else currentNumber++;

            SetColumnDigit();
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
        }
    }
}
