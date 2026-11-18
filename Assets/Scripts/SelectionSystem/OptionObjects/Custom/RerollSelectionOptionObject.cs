using Combat.IngameUI.Inspection.InspectionCanvas;
using Combat.IngameUI.Inspection;
using Cysharp.Threading.Tasks;
using Zenject;

public sealed class RerollSelectionOptionObject : SelectionOptionObject
{
    [Inject] private SelectionManager _selectionManager;
    private InspectionCanvasBase _inspectionCanvasBase;

    public void Initialize()
    {
        _inspectionCanvasBase = InspectionCanvasManager.Instance.OpenRerollPreview(transform);
    }
    
    public override void ApplySelectedEffect()
    {
        _selectionManager.RerollCurrentSelection().Forget();
    }

    private void OnDestroy()
    {
        InspectionCanvasManager.Instance.DestroyElement(_inspectionCanvasBase).Forget();
    }
}