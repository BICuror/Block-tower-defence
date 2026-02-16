using UnityEngine.UI;
using UnityEngine;
using TMPro;

public sealed class EffectInspectionTooltipPreview : InspectionPanelBase
{
    [SerializeField] private TextMeshProUGUI _previewName;
    [SerializeField] private Image _previewImage;
    private EntityModificatorData _entityModificatorData;
    
    public void Initialize(EntityModificatorData entityModificatorData, Transform target)
    {
        _entityModificatorData = entityModificatorData;   
        
        InitializeInspectionPanelBase(target.GetComponent<InspectableObject>());
        
        _previewImage.sprite = entityModificatorData.Icon;
    }
    
    protected override void UpdateAllParsableText()
    {
        _previewName.text = ParseTextByDefault(_entityModificatorData.ModificatorName);
    }
}