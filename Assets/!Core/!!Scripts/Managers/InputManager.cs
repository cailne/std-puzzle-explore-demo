using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Lucielle
{
    public class InputManager : Singleton<InputManager>
    {
        public LinkedList<InputLayerHandler> inputLayersStack = new();

        public Dictionary<InputActionSO, LinkedList<InputActionHandler>> inputActionStack = new();

        public InputLayerHandler currentInputLayer => inputLayersStack.Count > 0 ? inputLayersStack.Last.Value : null;

        private static List<InputLayerHandler> pendingInputLayers = new();
        private static List<InputActionHandler> pendingActionHandlers = new();

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < pendingInputLayers.Count; i++)
            {
                AddInputLayer(pendingInputLayers[i]);
            }
            pendingInputLayers.Clear();

            for (int i = 0; i < pendingActionHandlers.Count; i++)
            {
                AddInputActionHandler(pendingActionHandlers[i]);
            }
            pendingActionHandlers.Clear();
        }

        public static void AddInputLayer(InputLayerHandler inputLayerHandler)
        {
            if (inputLayerHandler == null) return;
            if (Instance == null)
            {
                pendingInputLayers.Add(inputLayerHandler);
                return;
            }

            Instance.AddLayerToStack(inputLayerHandler);
        }

        public static void RemoveInputLayer(InputLayerHandler inputLayerHandler)
        {
            if (inputLayerHandler == null) return;
            if (Instance == null)
            {
                pendingInputLayers.Remove(inputLayerHandler);
                return;
            }

            Instance.RemoveLayerFromStack(inputLayerHandler);
        }

        public static void AddInputActionHandler(InputActionHandler inputActionHandler)
        {
            if (inputActionHandler == null) return;
            if (Instance == null)
            {
                pendingActionHandlers.Add(inputActionHandler);
                return;
            }

            Instance.AddInputActionToStack(inputActionHandler);
        }

        public static void RemoveInputActionHandler(InputActionHandler inputActionHandler)
        {
            if (inputActionHandler == null) return;
            if (Instance == null)
            {
                pendingActionHandlers.Remove(inputActionHandler);
                return;
            }

            Instance.RemoveInputActionFromStack(inputActionHandler);
        }

        public static bool IsInputOnTop(InputActionHandler inputActionHandler)
        {
            if (Instance == null || inputActionHandler == null) return false;
            return Instance.IsInputActionOnTop(inputActionHandler);
        }

        private void AddLayerToStack(InputLayerHandler inputLayerHandler)
        {
			if (inputLayerHandler == null) return;
			if (inputLayersStack.Contains(inputLayerHandler)) inputLayersStack.Remove(inputLayerHandler);

			if (inputLayersStack.Count > 0) currentInputLayer?.DisableInput();

            inputLayersStack.AddLast(inputLayerHandler);
            inputLayerHandler.EnableInput();
        }

        private void RemoveLayerFromStack(InputLayerHandler inputLayerHandler)
        {
            if (inputLayerHandler == null) return;
            if (inputLayersStack.Count == 0) return;

            inputLayerHandler.DisableInput();
            inputLayersStack.Remove(inputLayerHandler);

            if (inputLayersStack.Count <= 0) return;
            InputLayerHandler topLayer = inputLayersStack.Last.Value;
            topLayer.EnableInput();
        }

        private void AddInputActionToStack(InputActionHandler inputActionHandler)
        {
            if (inputActionHandler == null || inputActionHandler.inputAction == null) return;
            RemoveInputActionFromStack(inputActionHandler);
            InputActionSO inputAction = inputActionHandler.inputAction;
            if (!inputActionStack.ContainsKey(inputAction)) inputActionStack[inputAction] = new();

            if (inputActionStack[inputAction].Contains(inputActionHandler)) return;
            inputActionStack[inputAction].AddLast(inputActionHandler);
        }

        private void RemoveInputActionFromStack(InputActionHandler inputActionHandler)
        {
            if (inputActionHandler == null || inputActionHandler.inputAction == null) return;
            InputActionSO inputAction = inputActionHandler.inputAction;
            if (!inputActionStack.ContainsKey(inputAction)) return;

            if (!inputActionStack[inputAction].Contains(inputActionHandler)) return;
            inputActionStack[inputAction].Remove(inputActionHandler);
        }

        private bool IsInputActionOnTop(InputActionHandler inputActionHandler)
        {
            if (inputActionHandler == null || inputActionHandler.inputAction == null) return false;
            InputActionSO inputAction = inputActionHandler.inputAction;

            if (!inputActionStack.ContainsKey(inputAction)) return false;
            return inputActionStack[inputAction].Count != 0 && inputActionStack[inputAction].Last.Value == inputActionHandler;
        }

#if UNITY_EDITOR
        [Button]
        public void CheckFrontLayer()
        {
            DebugManager.Log($"Current Layer Input Object: {currentInputLayer.gameObject.name}");
        }
#endif
    }
}
