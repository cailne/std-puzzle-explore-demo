using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Lucielle
{
    [Serializable]
    public class BoolChannelResponder : BaseChannelResponder
    {
        [SerializeField, HideIf(nameof(multipleChannel))]
        private BoolEventChannelSO eventChannelSO;

        [SerializeField, ShowIf(nameof(multipleChannel))]
        private List<BoolEventChannelSO> eventChannelSOList;

        [SerializeField] private bool invertResponse = false;
        [SerializeField] private UnityEvent onTrueRespond;
        [SerializeField] private UnityEvent onFalseRespond;

        public void PlayResponse(bool isActive)
        {
            if (invertResponse) isActive = !isActive;

            if (isActive) onTrueRespond?.Invoke();
            else onFalseRespond?.Invoke();
        }

        public override void RegisterListener()
        {
            if (multipleChannel)
            {
                foreach (BoolEventChannelSO eventChannel in eventChannelSOList)
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
                foreach (BoolEventChannelSO eventChannel in eventChannelSOList)
                {
                    eventChannel.RemoveListener(PlayResponse);
                }
            }
            else eventChannelSO.RemoveListener(PlayResponse);
        }
    }
}
