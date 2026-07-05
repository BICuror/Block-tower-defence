using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

[RequireComponent(typeof(TooltipDataParser))]
[RequireComponent(typeof(VisualTextParser))]

public abstract class InspectionPanelBase : ParserableTextContainer
{
    [SerializeField] private PointFollowingUIElement _pointFollowingUIElement;
    [SerializeField] private UIElementFadeAnimator _uiElementFadeAnimator;
    [SerializeField] private HoverableUIElement _hoverableUIElement;
    private InspectableObject _inspectable;

    protected InspectableObject Inspectable => _inspectable;
    public bool IsHoveredOver => _hoverableUIElement.PointerHoveredOver;
    public bool IsActive => _uiElementFadeAnimator.IsActive;
    
    public async UniTask Enable() => await _uiElementFadeAnimator.Enable();
    public async UniTask Disable() => await _uiElementFadeAnimator.Disable();

    protected void SetDynamicOffsetProvider(Func<Vector2> dynamicOffsetProvider) => _pointFollowingUIElement.SetDynamicOffsetProvider(dynamicOffsetProvider);
    
    protected void InitializeInspectionPanelBase(InspectableObject inspectableObject)
    {
        _uiElementFadeAnimator.SetAlpha(0f);
        
        _inspectable = inspectableObject;
        
        _pointFollowingUIElement.SetTarget(inspectableObject.transform);
        
        SetParsers(inspectableObject.ReplaceableDataParser, GetComponent<VisualTextParser>(), GetComponent<TooltipDataParser>());
    }
}