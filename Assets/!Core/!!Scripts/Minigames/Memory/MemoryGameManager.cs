using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Lucielle
{
    public class MemoryGameManager : MonoBehaviour
    {
        private const int MIN_CARD_COUNT = 12;

        [Header("Modules")]
        [SerializeField] private Transform cardContainer;
        [SerializeField] private MemoryCard cardPrefab;

        [Space(5f), Header("Channels")]
        [SerializeField] private IntEventChannelSO cardClickEventChannelSO;

        private List<MemoryCard> pooledCards = new();
        private bool isStarted = false;

        public void Initialize()
        {
            if (pooledCards.Count >= MIN_CARD_COUNT) return;

            for (int i = 0; i < MIN_CARD_COUNT; i++)
            {
                MemoryCard card = Instantiate(cardPrefab, cardContainer);
                card.gameObject.SetActive(false);
                pooledCards.Add(card);
            }
        }

        [Button]
        public void StartMemoryGame(int startingCount = MIN_CARD_COUNT)
        {
            if (startingCount < MIN_CARD_COUNT) return;
            if (startingCount % 2 != 0) return;

            List<int> dummyIndexes = new();
            int contentMax = startingCount / 2;

            for (int i = 0; i < contentMax; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    dummyIndexes.Add(i);
                }
            }

            ShuffleList(dummyIndexes);

            for (int i = 0; i < startingCount; i++)
            {
                if (i >= pooledCards.Count)
                {
                    MemoryCard card = Instantiate(cardPrefab, cardContainer);
                    card.gameObject.SetActive(false);
                    pooledCards.Add(card);
                }

                pooledCards[i].Initialize(dummyIndexes[i]);
                pooledCards[i].gameObject.SetActive(true);
            }
            return;

            void ShuffleList(List<int> list)
            {
                for (int i = list.Count - 1; i > 0; i--)
                {
                    int randomIndex = Random.Range(0, i + 1);
                    (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
                }
            }
        }

        public void ResetGame()
        {
            foreach (MemoryCard card in pooledCards)
            {
                card.gameObject.SetActive(false);
            }
        }
    }
}
