using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lucielle
{
    public class SendToTopUIBehaviour : MonoBehaviour, IPointerDownHandler
    {
        public bool isActive { get; set; } = true;
        public event Action onSendToTop;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isActive) return;
            SendToTop();
        }

        public void SendToTop()
        {
            this.transform.SetAsLastSibling();
            onSendToTop?.Invoke();
        }
    }
}
