using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lucielle
{
    public class InputActionHandler : MonoBehaviour
    {
        [SerializeField] private bool autoEnable = true;
        [SerializeField] private bool onlyTriggerWhenTopAction = false;
        public InputActionSO inputAction;

        [SerializeField] private InvokeTarget invokeTarget = InvokeTarget.Button;

        [SerializeField, ShowIf(nameof(invokeTarget), InvokeTarget.Button)]
        private Button targetButton;

        [SerializeField, ShowIf(nameof(invokeTarget), InvokeTarget.Events)]
        public UnityEvent onInputAction;

        [SerializeField, ShowIf(nameof(invokeTarget), InvokeTarget.Events)]
        public UnityEvent onInputCanceledAction;

        [Header("Debug")]
        [ReadOnly, ShowNonSerializedField] private bool isActive = false;
        [ReadOnly] private InputLayerHandler inputLayerHandler;

        private List<CanvasGroup> canvasGroups = new();
        [ReadOnly] private static List<InputActionHandler> lastInvokedInputActions = new();

        private SendToTopUIBehaviour sendToTopUIBehaviour;
        [ShowNonSerializedField] private bool isLastInvoked;

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            inputLayerHandler?.UnregisterInputActionHandler(this);
            if (sendToTopUIBehaviour != null) sendToTopUIBehaviour.onSendToTop -= AddInputToStack;
        }

        private void OnEnable()
        {
            AddInputToStack();
            if (autoEnable) EnableInput();
        }

        private void OnDisable()
        {
            DisableInput();
            RemoveInputFromStack();
        }

        private void Update()
        {
            HandleInput();
        }

        private void LateUpdate()
        {
            lastInvokedInputActions.Remove(this);
        }

        private void Initialize()
        {
            if (invokeTarget == InvokeTarget.Button && targetButton == null)
                targetButton = GetComponent<Button>();

            canvasGroups.AddRange(GetComponentsInParent<CanvasGroup>(true));
            inputLayerHandler = GetComponentInParent<InputLayerHandler>();
            inputLayerHandler?.RegisterInputActionHandler(this);

            sendToTopUIBehaviour = GetComponentInParent<SendToTopUIBehaviour>();
            if (sendToTopUIBehaviour != null)
                sendToTopUIBehaviour.onSendToTop += AddInputToStack;
        }

        public void EnableInput()
        {
            isActive = true;
            isLastInvoked = false;
        }

        public void DisableInput()
        {
            isActive = false;
        }

        private void AddInputToStack()
        {
			InputManager.AddInputActionHandler(this);
        }

        private void RemoveInputFromStack()
        {
            InputManager.RemoveInputActionHandler(this);
        }

        private void HandleInput()
        {
            if (!isActive) return;
            if (inputAction == null || !CanTriggerAction()) return;

            if (IsBlockedByCanvasGroup()) return;
            if (!inputAction.CheckForInput())
            {
                if (inputAction.IsInputTypeHold() && isLastInvoked)
                    onInputCanceledAction?.Invoke();
                isLastInvoked = false;
                return;
            }

            switch (invokeTarget)
            {
                case InvokeTarget.Button:
                    InvokeButtonClick();
                    break;
                case InvokeTarget.Events:
                    InvokeEvent();
                    break;
            }

            isLastInvoked = true;
            lastInvokedInputActions.Add(this);
        }

        private bool CanTriggerAction()
        {
            if (!onlyTriggerWhenTopAction) return true;

            bool isTopAction = !onlyTriggerWhenTopAction || InputManager.IsInputOnTop(this);

            return isTopAction;
        }

        private bool IsBlockedByCanvasGroup()
        {
            bool canvasNull = false;
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                if (canvasGroup == null)
                {
                    canvasNull = true;
                    break;
                }
                if (!canvasGroup.interactable || !canvasGroup.blocksRaycasts) return true;
                if (canvasGroup.ignoreParentGroups) break;
            }

            if (!canvasNull) return false;
            ReinitializeCanvasGroup();
            return IsBlockedByCanvasGroup();
        }

        private void ReinitializeCanvasGroup()
        {
            canvasGroups.Clear();
            canvasGroups.AddRange(GetComponentsInParent<CanvasGroup>(true));
        }

        private void InvokeButtonClick()
        {
            if (targetButton == null) return;
            if (!targetButton.interactable || !targetButton.gameObject.activeInHierarchy) return;
            targetButton.onClick.Invoke();
        }

        private void InvokeEvent()
        {
            onInputAction?.Invoke();
        }
    }

    public enum InvokeTarget
    {
        Button,
        Events,
    }
}
