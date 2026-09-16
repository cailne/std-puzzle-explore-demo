using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Lucielle
{
    [Serializable]
    public class KeyInput
    {
        public bool useKeyCode = true;
        [ShowIf(nameof(useKeyCode))] public KeyCode keyCode;
        [HideIf(nameof(useKeyCode))] public string keyName = string.Empty;
        public KeyInputType inputType = KeyInputType.Down;
        public UnityEvent onKeyInputEvent;

        public void CheckForInput()
        {
            if (keyCode == KeyCode.None) return;
            switch (inputType)
            {
                case KeyInputType.Hold:
                    if (GetKeyHold()) InvokeKeyInputEvent();
                    break;
                case KeyInputType.Down:
                    if (GetKeyDown()) InvokeKeyInputEvent();
                    break;
                case KeyInputType.Up:
                    if (GetKeyUp()) InvokeKeyInputEvent();
                    break;
            }
        }

        private bool GetKeyDown()
        {
            return !useKeyCode ? Input.GetKeyDown(keyName) : Input.GetKeyDown(keyCode);
        }

        private bool GetKeyHold()
        {
            return !useKeyCode ? Input.GetKey(keyName) : Input.GetKey(keyCode);
        }

        private bool GetKeyUp()
        {
            return !useKeyCode ? Input.GetKeyUp(keyName) : Input.GetKeyUp(keyCode);
        }

        private void InvokeKeyInputEvent()
        {
            onKeyInputEvent?.Invoke();
        }
    }

    public enum KeyInputType
    {
        Hold,
        Down,
        Up
    }
}
