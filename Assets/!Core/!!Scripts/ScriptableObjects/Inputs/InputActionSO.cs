using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Lucielle
{
    [CreateAssetMenu(fileName = "_InputActionSO", menuName = "Scriptable Objects/Utils/InputActionSO")]
    public class InputActionSO : DescriptionSO
    {
        public string actionName;

        public bool useKeyCode = false;
        public KeyInputType inputType = KeyInputType.Down;
        [ShowIf(nameof(useKeyCode))] public KeyCode keyCode = KeyCode.None;
        [HideIf(nameof(useKeyCode))] public string keyName = string.Empty;

        public bool CheckForInput()
        {
            return inputType switch
            {
                KeyInputType.Hold => GetInputHold(),
                KeyInputType.Down => GetInputDown(),
                KeyInputType.Up => GetInputUp(),
                _ => false
            };
        }

        public bool IsInputTypeHold()
        {
            return inputType == KeyInputType.Hold;
        }

        private bool GetInputDown()
        {
            return !useKeyCode ? Input.GetKeyDown(keyName) : Input.GetKeyDown(keyCode);
        }

        private bool GetInputUp()
        {
            return !useKeyCode ? Input.GetKeyUp(keyName) : Input.GetKeyUp(keyCode);
        }

        private bool GetInputHold()
        {
            return !useKeyCode ? Input.GetKey(keyName) : Input.GetKey(keyCode);
        }
    }
}
