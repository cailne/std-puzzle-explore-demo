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

            IInteractable obj = other.GetComponent<IInteractable>();
            if (obj != null)
                interactable = obj;
        }

        private void OnTriggerExit(Collider other)
        {
            interactable = null;
        }

        public void CallInteract()
        {
            if (interactable == null) return;
            interactable.Interact();
        }
    }
}
