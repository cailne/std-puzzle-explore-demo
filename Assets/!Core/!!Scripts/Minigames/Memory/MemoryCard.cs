using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Lucielle
{
    public class MemoryCard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI cardText;
        [SerializeField] private IntEventChannelSO cardClickEventChannelSO;
        private int currentIndex;
        private bool isFlipped;
        private bool complete = false;

        public int CurrentIndex => currentIndex;
        public bool IsFlipped => isFlipped;
        public bool IsComplete => complete;

        public void Initialize(int index)
        {
            currentIndex = index;
            isFlipped = false;
            complete = false;
            if(cardText != null)
                cardText.text = currentIndex.ToString();
        }

        public void Flip()
        {
            if (complete) return;
            if (isFlipped) return;
            isFlipped = true;
            cardClickEventChannelSO.RaiseEvent(currentIndex);
        }

        public void Unflip()
        {
            if (complete) return;
            if (!isFlipped) return;
            isFlipped = false;
        }

        public void Complete()
        {
            complete = true;
            isFlipped = true;
        }

        public void ResetCard()
        {
            isFlipped = false;
        }
    }
}
