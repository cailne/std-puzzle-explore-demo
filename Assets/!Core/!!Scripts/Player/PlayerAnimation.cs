using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucielle
{
    public class PlayerAnimation : MonoBehaviour
    {
        private static readonly int Speed = Animator.StringToHash("Speed");

        [SerializeField] private Animator animator;

        public void SetAnimationFloat(float value)
        {
            animator.SetFloat(Speed, value);
        }
    }
}
