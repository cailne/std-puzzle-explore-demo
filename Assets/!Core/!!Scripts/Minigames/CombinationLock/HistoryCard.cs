using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lucielle
{
    public class HistoryCard : MonoBehaviour
    {
        [SerializeField] private List<Color> legendColors = new();
        [SerializeField] private List<Image> legendImages = new();

        public void Initialize(List<COMBINATION_STATUS> combinationStatuses)
        {
            for (int i = 0; i < legendImages.Count; i++)
            {
                if (i >= combinationStatuses.Count)
                {
                    legendImages[i].color = legendColors[(int)COMBINATION_STATUS.NOTHING];
                    continue;
                }
                legendImages[i].color = legendColors[(int)combinationStatuses[i]];
            }
        }

        public void Initialize(COMBINATION_STATUS c1, COMBINATION_STATUS c2, COMBINATION_STATUS c3)
        {
            legendImages[0].color = legendColors[(int)c1];
            legendImages[1].color = legendColors[(int)c2];
            legendImages[2].color = legendColors[(int)c3];
        }
    }

    public enum COMBINATION_STATUS
    {
        NOTHING = 0,
        CLOSE = 1,
        RIGHT = 2,
    }
}
