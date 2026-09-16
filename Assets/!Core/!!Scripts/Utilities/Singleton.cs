using UnityEngine;

namespace Lucielle
{
    [DisallowMultipleComponent]
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));
                    if (instance == null)
                        CreateInstance();
                }

                return instance;
            }
        }

        private static void CreateInstance()
        {
            var singleton = new GameObject();
            instance = singleton.AddComponent<T>();
            singleton.name = $"(Singleton) {typeof(T)}";

            var component = instance.GetComponent<Singleton<T>>();
            component.OnCreate();

            if (component.IsPersistBetweenScenes) DontDestroyOnLoad(singleton);
        }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this as T;

            if (IsPersistBetweenScenes)
            {
                DontDestroyOnLoad(this);
            }
        }

        // Called after singleton is created
        protected virtual void OnCreate() { }

        // Decide if singleton persist between scenes or not
        protected virtual bool IsPersistBetweenScenes => true;

        protected virtual void OnDestroy()
        {
            if (instance == this) instance = null;
        }
    }
}
