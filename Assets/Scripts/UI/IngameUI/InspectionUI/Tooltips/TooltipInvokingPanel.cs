using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System;

public abstract class TooltipInvokingPanel : ParserableTextContainer, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] private List<ContentSizeFitter> _contentSizeFitters;
    
    protected abstract TooltipParseTagDataContainer TagDataContainer {get;}
    
    public Action<TooltipParseTagDataContainer> TooltipOpened;
    public Action TooltipClosed;
    
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => TooltipOpened.Invoke(TagDataContainer);
    
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => TooltipClosed.Invoke();
    
    protected void UpdateContentSizeFilters()
    {
        _contentSizeFitters.ForEach(contentSizeFitter =>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentSizeFitter.transform as RectTransform);
        });
    }
}