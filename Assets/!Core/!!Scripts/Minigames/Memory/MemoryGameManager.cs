using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lucielle
{
    public class MemoryGameManager : MonoBehaviour
    {
        private const int MIN_CARD_COUNT = 12;

        [Header("Modules")]
        [SerializeField] private GameObject gameParent;
        [SerializeField] private Transform cardContainer;
        [SerializeField] private MemoryCard cardPrefab;
        [SerializeField] private GenericDatabaseSO cardDatabase;

        [Space(5f), Header("Channels")]
        [SerializeField] private IntEventChannelSO cardClickEventChannelSO;
        [SerializeField] private VoidEventChannelSO memoryGameStartEventChannelSO;

        private List<MemoryCard> pooledCards = new();
        private bool isStarted = false;
        private int previousIndex = -1;

        private void Awake()
        {
            Initialize();
            cardClickEventChannelSO?.RegisterListener(OnCardInteract);
            memoryGameStartEventChannelSO?.RegisterListener(() => StartMemoryGame());
        }

        private void OnDestroy()
        {
            cardClickEventChannelSO?.RemoveListener(OnCardInteract);
            memoryGameStartEventChannelSO?.RemoveListener(() => StartMemoryGame());
        }

        public void Initialize()
        {
            if (pooledCards.Count >= MIN_CARD_COUNT) return;

            for (int i = 0; i < MIN_CARD_COUNT; i++)
            {
                MemoryCard card = Instantiate(cardPrefab, cardContainer);
                card.gameObject.SetActive(false);
                pooledCards.Add(card);
            }

            previousIndex = -1;
            gameParent.SetActive(false);
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

                int cardIndex = dummyIndexes[i];
                pooledCards[i].Initialize(cardIndex, cardDatabase.GetObjectAsset<SpriteSO>(cardIndex));
                pooledCards[i].gameObject.SetActive(true);
            }

            gameParent.SetActive(true);
            previousIndex = -1;
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

        private void OnCardInteract(int index)
        {
            if (previousIndex < 0)
            {
                previousIndex = index;
                return;
            }

            if (previousIndex == index)
            {
                var relatedCard = GetCard(index);
                relatedCard.Item1.Complete();
                relatedCard.Item2.Complete();

                previousIndex = -1;
                //check the game is finished or not
                if (!CheckGameState()) return;

                gameParent.SetActive(false);
                ResetGame();
            }
            else
            {
                ResetCard();
                previousIndex = -1;
            }
        }

        private (MemoryCard, MemoryCard) GetCard(int index)
        {
            (MemoryCard, MemoryCard) result = (null, null);
            foreach (MemoryCard card in pooledCards)
            {
                if (card.CurrentIndex != index) continue;

                if (result.Item1 == null)
                {
                    result.Item1 = card;
                    continue;
                }
                result.Item2 = card;
                break;
            }

            return result;
        }

        private void ResetCard()
        {
            foreach (MemoryCard card in pooledCards)
            {
                if (card.IsFlipped)
                    card.Unflip();
            }
        }

        //return true if the game is finished
        private bool CheckGameState()
        {
            return pooledCards.All(card => card.IsComplete);
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
