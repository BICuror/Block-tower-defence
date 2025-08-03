using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public sealed class EffectInspectionTooltipPreview : PointFollowingCanvasUIElement
{
    [SerializeField] private TextMeshProUGUI _previewName;
    [SerializeField] private Image _previewImage;
    
    public async UniTask Initialilize(SelectionOptionObject selectionOptionObject)
    {
        _previewName.text = selectionOptionObject.OptionName;
        _previewImage.sprite = selectionOptionObject.Icon;
        SetTarget(selectionOptionObject.transform);
        await RebuildLayoutAndCalculateOffsets();
    }
}