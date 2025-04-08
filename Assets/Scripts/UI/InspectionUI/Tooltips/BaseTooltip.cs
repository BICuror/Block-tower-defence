using UnityEngine.EventSystems;
using UnityEngine;
using System;

public abstract class BaseTooltip : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    protected abstract TooltipParseTagDataContainer TagDataContainer {get;}
    
    public Action<TooltipParseTagDataContainer> TooltipOpened;
    public Action TooltipClosed;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipOpened.Invoke(TagDataContainer);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipClosed.Invoke();
    }
}