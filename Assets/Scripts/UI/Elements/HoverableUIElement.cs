using UnityEngine.EventSystems;
using UnityEngine;

public sealed class HoverableUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool _pointerHoveredOver;
    
    public bool PointerHoveredOver => _pointerHoveredOver;
    
    void IPointerEnterHandler.OnPointerEnter(PointerEventData _) => _pointerHoveredOver = true;
    
    void IPointerExitHandler.OnPointerExit(PointerEventData _) => _pointerHoveredOver = false;
}