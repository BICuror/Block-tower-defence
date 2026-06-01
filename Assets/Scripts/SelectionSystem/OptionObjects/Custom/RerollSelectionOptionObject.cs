using Cysharp.Threading.Tasks;
using Zenject;

public sealed class RerollSelectionOptionObject : SelectionOptionObject
{
    [Inject] private SelectionManager _selectionManager;
    private InspectionPanelBase _inspectionPanelBase;

    public void Initialize()
    {
        _inspectionPanelBase = InspectionTooltipManager.Instance.OpenRerollPreview(transform);
    }
    
    public override void ApplySelectedEffect()
    {
        _selectionManager.RerollCurrentSelection().Forget();
    }

    private void OnDestroy()
    {
        InspectionTooltipManager.Instance.DestroyElement(_inspectionPanelBase).Forget();
    }
}