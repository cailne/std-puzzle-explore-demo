using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucielle
{
    [CreateAssetMenu(fileName = "SpriteSO", menuName = "Scriptable Objects/Generic/SpriteSO")]
    public class SpriteSO : DatabaseableSO
    {
        [SerializeField] private Sprite cardFrontSprite;
        [SerializeField] private Sprite cardBackSprite;

        public Sprite CardFrontSprite => cardFrontSprite;
        public Sprite CardBackSprite => cardBackSprite;
    }
}


