using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lucielle
{
    public class MemoryCard : MonoBehaviour
    {
        [Header("Modules")]
        [SerializeField] private Image cardImage;
        [SerializeField] private SpriteSO cardSprite;
        [Header("Channels")]
        [SerializeField] private IntEventChannelSO cardClickEventChannelSO;
        [SerializeField] private BoolEventChannelSO cardAnimationDoneEventChannelSO;
        private int currentIndex;
        private bool isFlipped;
        private bool complete = false;
        private bool isPlayAllowed = false;

        private Sequence cardFlipSequence;
        private Sequence cardUnflipSequence;

        public int CurrentIndex => currentIndex;
        public bool IsFlipped => isFlipped;
        public bool IsComplete => complete;

        private void OnEnable()
        {
            cardAnimationDoneEventChannelSO?.RegisterListener(SetPlayState);
        }

        private void OnDisable()
        {
            cardAnimationDoneEventChannelSO?.RemoveListener(SetPlayState);
        }

        public void Initialize(int index, SpriteSO cardSprite)
        {
            currentIndex = index;
            isFlipped = false;
            complete = false;
            isPlayAllowed = true;
            this.cardSprite = cardSprite;
            cardImage.sprite = cardSprite.CardBackSprite;
        }

        public void Flip()
        {
            if (!isPlayAllowed) return;
            if (complete) return;
            if (isFlipped) return;

            //start flip animation here
            cardAnimationDoneEventChannelSO?.RaiseEvent(false);
            cardFlipSequence = DOTween.Sequence();
            cardFlipSequence.Append(cardImage.rectTransform.DOScaleX(0.01f, 0.2f)
                .From(1f)
                .SetEase(Ease.InCubic).OnComplete(() =>
                {
                    cardImage.sprite = cardSprite.CardFrontSprite;
                }));
            cardFlipSequence.Append(cardImage.rectTransform.DOScaleX(1f, 0.3f).From(0.01f).SetEase(Ease.OutCubic));
            cardFlipSequence.OnComplete(FlipAnimationDone);
            cardFlipSequence.Play();
        }

        public void Unflip()
        {
            if (complete) return;
            if (!isFlipped) return;

            cardAnimationDoneEventChannelSO?.RaiseEvent(false);
            cardUnflipSequence = DOTween.Sequence();
            cardUnflipSequence.Append(cardImage.rectTransform.DOScaleX(0.01f, 0.2f)
                .From(1f)
                .SetEase(Ease.InCubic).OnComplete(() =>
                {
                    cardImage.sprite = cardSprite.CardBackSprite;
                }));
            cardUnflipSequence.Append(cardImage.rectTransform.DOScaleX(1f, 0.3f).From(0.01f).SetEase(Ease.OutCubic));
            cardUnflipSequence.OnComplete(UnflipAnimationDone);
            cardUnflipSequence.Play();
        }

        public void Complete()
        {
            complete = true;
            isFlipped = true;
            cardImage.sprite = cardSprite.CardFrontSprite;
        }

        public void ResetCard()
        {
            isFlipped = false;
            cardSprite = null;
            cardImage.sprite = null;
            isPlayAllowed = false;
        }

        private void SetPlayState(bool allowed)
        {
            isPlayAllowed = allowed;
        }

        private void FlipAnimationDone()
        {
            isFlipped = true;
            cardImage.sprite = cardSprite.CardFrontSprite;
            cardClickEventChannelSO.RaiseEvent(currentIndex);
            cardAnimationDoneEventChannelSO?.RaiseEvent(true);
        }

        private void UnflipAnimationDone()
        {
            isFlipped = false;
            cardImage.sprite = cardSprite.CardBackSprite;
            cardAnimationDoneEventChannelSO?.RaiseEvent(true);
        }
    }
}
