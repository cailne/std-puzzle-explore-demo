using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Lucielle
{
    [Serializable]
    public class ChannelResponder : BaseChannelResponder
    {
        [SerializeField, HideIf(nameof(multipleChannel))]
        private BaseEventChannelSO eventChannelSO;

        [SerializeField, ShowIf(nameof(multipleChannel))]
        private List<BaseEventChannelSO> eventChannelSOList;

        [SerializeField] private UnityEvent eventResponses;

        public void PlayResponse()
        {
            eventResponses?.Invoke();
        }

        public override void RegisterListener()
        {
            if (multipleChannel)
            {
                foreach (BaseEventChannelSO eventChannel in eventChannelSOList)
                {
                    eventChannel.RegisterListener(PlayResponse);
                }
            }
            else eventChannelSO.RegisterListener(PlayResponse);
        }

        public override void RemoveListener()
        {
            if (multipleChannel)
            {
                foreach (BaseEventChannelSO eventChannel in eventChannelSOList)
                {
                    eventChannel.RemoveListener(PlayResponse);
                }
            }
            else eventChannelSO.RemoveListener(PlayResponse);
        }
    }
}
