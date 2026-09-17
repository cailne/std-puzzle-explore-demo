using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucielle
{
    public class MemoryCard : MonoBehaviour
    {
        [SerializeField] private IntEventChannelSO cardClickEventChannelSO;
        private int currentIndex;
        private bool isFlipped;

        public int CurrentIndex => currentIndex;

        public void Initialize(int index)
        {
            currentIndex = index;
            isFlipped = false;
        }

        public void Flip()
        {
            if (isFlipped) return;
            isFlipped = true;
            cardClickEventChannelSO.RaiseEvent(currentIndex);
        }

        public void ResetCard()
        {
            isFlipped = false;
        }
    }
}
