using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Lucielle
{
    public class EventChannelResponder : MonoBehaviour
    {
        [SerializeField] private bool registerOnAwake = false;
        [SerializeField] private List<ChannelResponder> eventResponder;
        [SerializeField] private List<BoolChannelResponder> boolEventResponder;

        [Button]
        private void ReInitialize()
        {
            if (!gameObject.activeInHierarchy) return;
            RemoveListener();
            RegisterListener();
        }

        private void Awake()
        {
            if (!registerOnAwake) return;
            RegisterListener();
        }

        private void OnEnable()
        {
            if (registerOnAwake) return;
            RegisterListener();
        }

        private void OnDisable()
        {
            if (registerOnAwake) return;
            RemoveListener();
        }

        private void OnDestroy()
        {
            if (!registerOnAwake) return;
            RemoveListener();
        }

        private void RegisterListener()
        {
            foreach (ChannelResponder channelResponder in eventResponder)
                channelResponder.RegisterListener();

            foreach (BoolChannelResponder channelResponder in boolEventResponder)
                channelResponder.RegisterListener();
        }

        private void RemoveListener()
        {
            foreach (ChannelResponder channelResponder in eventResponder)
                channelResponder.RemoveListener();

            foreach (BoolChannelResponder channelResponder in boolEventResponder)
                channelResponder.RemoveListener();
        }
    }
}
