using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucielle
{
    public class InteractableHandler : MonoBehaviour, IInteractable
    {
        [SerializeField] private VoidEventChannelSO voidEventChannel;

        public void Interact()
        {
            voidEventChannel?.RaiseEvent();
        }
    }
}
