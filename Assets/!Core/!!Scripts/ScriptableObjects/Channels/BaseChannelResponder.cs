using System;
using UnityEngine;

namespace Lucielle
{
    [Serializable]
    public abstract class BaseChannelResponder
    {
        [TextArea] public string notes;

        [SerializeField] protected bool multipleChannel = false;

        public abstract void RegisterListener();
        public abstract void RemoveListener();
    }
}
