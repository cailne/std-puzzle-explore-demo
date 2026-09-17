using UnityEngine;

namespace Lucielle
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private VoidEventChannelSO interactEventChannelSO;
        private IInteractable interactable;

        private void OnEnable()
        {
            interactEventChannelSO?.RegisterListener(CallInteract);
        }

        private void OnDisable()
        {
            interactEventChannelSO?.RemoveListener(CallInteract);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (interactable != null) return;
            if (!other.gameObject.tag.Equals("Interactable")) return;

            IInteractable obj = other.GetComponent<IInteractable>();
            if (obj != null)
                interactable = obj;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.tag.Equals("Interactable")) return;
            interactable = null;
        }

        private void CallInteract()
        {
            interactable?.Interact();
        }
    }
}
