using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public sealed class EffectInspectionTooltipPreview : PointFollowingCanvasUIElement
{
    [SerializeField] private TextMeshProUGUI _previewName;
    [SerializeField] private Image _previewImage;
    
    public void Initialilize(EntityModificatorData entityModificatorData, Transform target)
    {
        _previewName.text = entityModificatorData.ModificatorName;
        _previewImage.sprite = entityModificatorData.Icon;
        SetTarget(target);
    }
}