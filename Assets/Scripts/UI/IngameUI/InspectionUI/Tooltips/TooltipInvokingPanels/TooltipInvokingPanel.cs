using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace Combat.IngameUI.Inspection.Tooltips
{
    public abstract class TooltipInvokingPanel : ParsableTextContainer, IPointerExitHandler, IPointerEnterHandler
    {
        [SerializeField] private List<ContentSizeFitter> _contentSizeFitters;

        public abstract TooltipParseTagDataContainer TagDataContainer { get; }

        public event Action<TooltipInvokingPanel> TooltipOpened;
        public event Action<TooltipInvokingPanel> TooltipClosed;
        
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => TooltipOpened?.Invoke(this);

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => TooltipClosed?.Invoke(this);

        protected void UpdateContentSizeFilters()
        {
            _contentSizeFitters.ForEach(contentSizeFitter =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentSizeFitter.transform as RectTransform);
            });
        }
    }
}