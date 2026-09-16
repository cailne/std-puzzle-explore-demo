using UnityEngine;

namespace Lucielle
{
    public class ComponentReferenceHandler : MonoBehaviour
    {
        [SerializeField, TextArea] private string notes;
        [SerializeField] private ComponentReferenceSO componentReference;

        public bool hasSetReferenceObject = false;

        private void Awake()
        {
            SetReferenceObject();
        }

        private void OnDestroy()
        {
            UnsetReferenceObject();
        }

        public void SetReferenceObject()
        {
            if (hasSetReferenceObject) return;
            hasSetReferenceObject = true;
            componentReference.SetReferenceObject(gameObject, this);
        }

        public void UnsetReferenceObject()
        {
            componentReference.UnSetReferenceObject(gameObject);
            hasSetReferenceObject = false;
        }
    }
}
