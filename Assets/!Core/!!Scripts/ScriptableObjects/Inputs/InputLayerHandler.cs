using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Lucielle
{
    public class InputLayerHandler : MonoBehaviour
    {
        [SerializeField] private bool autoActivate = true;

        [BoxGroup("Debug"), ShowNonSerializedField, ReadOnly]
        private bool isActive = false;

        [BoxGroup("Debug"), SerializeField, ReadOnly]
        private List<InputActionHandler> inputActionHandlers = new();

        public bool IsActive => isActive;

        private void OnEnable()
        {
            if(autoActivate) AddToLayerStack();
        }

        private void OnDisable()
        {
            RemoveFromLayerStack();
        }

        public void AddToLayerStack()
        {
            isActive = true;
            InputManager.AddInputLayer(this);
        }

        public void RemoveFromLayerStack()
        {
            isActive = false;
            InputManager.RemoveInputLayer(this);
        }

        public void RegisterInputActionHandler(InputActionHandler inputActionHandler)
        {
            if (inputActionHandlers.Contains(inputActionHandler)) return;
            inputActionHandlers.Add(inputActionHandler);

            if(isActive) inputActionHandler.EnableInput();
            else inputActionHandler.DisableInput();
        }

        public void UnregisterInputActionHandler(InputActionHandler inputActionHandler)
        {
            if (!inputActionHandlers.Contains(inputActionHandler)) return;
            inputActionHandlers.Remove(inputActionHandler);
            inputActionHandler.DisableInput();
        }

        public void EnableInput()
        {
            isActive = true;
            foreach (InputActionHandler inputActionHandler in inputActionHandlers)
            {
                inputActionHandler.EnableInput();
            }
        }

        public void DisableInput()
        {
            isActive = false;
            foreach (InputActionHandler inputActionHandler in inputActionHandlers)
            {
                inputActionHandler.DisableInput();
            }
        }
    }
}
