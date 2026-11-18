using UnityEngine;

namespace Combat.IngameUI.Inspection.InspectionCanvas
{
    public abstract class ResizableInspectionCanvasBase : InspectionCanvasBase
    {
        [Header("Offset")]
        [SerializeField] private RectTransform _mainContentTransform;
        [SerializeField] private RectTransform _rootRectTransform;
        
        protected void InitializeResizableInspectionPanel(InspectableObject inspectableObject)
        {
            SetDynamicOffsetProvider(GetDynamicOffset);
            
            InitializeInspectionPanelBase(inspectableObject);
        }
        
        private Vector2 GetDynamicOffset() => new((_rootRectTransform.sizeDelta.x / 2f - _mainContentTransform.sizeDelta.x / 2f) * _rootRectTransform.transform.localScale.x, _mainContentTransform.sizeDelta.y);
    }
}