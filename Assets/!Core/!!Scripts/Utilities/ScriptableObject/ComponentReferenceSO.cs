using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lucielle
{
	[CreateAssetMenu(fileName = "ObjectReferenceSO", menuName = "Scriptable Objects/Editor/Component ReferenceSO")]
    public class ComponentReferenceSO : DescriptionSO
    {
        [SerializeField] private List<ComponentReferenceSO> fallbackReferenceList = new();

        private Dictionary<Type, Component> referenceComponents = new();
        private GameObject referenceObject;

        public void SetReferenceObject<T>(GameObject referenceObject = null, params T[] components) where T : Component
        {
            this.referenceObject = referenceObject;
            referenceComponents.Clear();
            foreach (T component in components)
            {
                AddReference(component);
            }
        }

        public void UnSetReferenceObject(GameObject gameObject)
        {
            if (referenceObject != gameObject) return;
            referenceObject = null;
            referenceComponents.Clear();
        }

        public void AddReference<T>(T component) where T : Component
        {
            if (!referenceComponents.ContainsKey(typeof(T)))
            {
                referenceComponents.Add(typeof(T), component);
            }
        }

        public void RemoveReference<T>() where T : Component
        {
            if (ContainsComponent<T>())
            {
                referenceComponents.Remove(typeof(T));
            }
        }

        public bool ContainsComponent<T>() where T : Component
        {
            return referenceComponents.ContainsKey(typeof(T));
        }

        private T GetComponentReference<T>() where T : Component
        {
            T component = referenceComponents[typeof(T)] as T;
            if (component != null) return component;
            foreach (ComponentReferenceSO fallbackComponent in fallbackReferenceList)
            {
                T fallback = fallbackComponent.GetComponent<T>();
                if (fallback != null) return fallback;
            }
            return null;
        }

        public T GetComponent<T>() where T : Component
        {
            if (referenceObject == null) return null;
            if (!ContainsComponent<T>()) AddReference(referenceObject.GetComponent<T>());
            return GetComponentReference<T>();
        }
    }
}
