using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public sealed class EffectInspectionTooltip : InspectionPanel, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private List<ContentSizeFitter> _contentSizeFitters;
    [SerializeField] private Image _effectIcon;
    [SerializeField] private TextMeshProUGUI _descriptionTextField;
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private TooltipTextParser _tooltipTextParser;
    [SerializeField] private TooltipDataParser _tooltipDataParser;
    [SerializeField] private InspectionSubpanelsController _inspectionSubpanelsController;
    [SerializeField] private PointFollowerUI _pointFollowerUI;
    private SelectionOptionObject _selectionOptionObject;

    public async UniTask SetSelectionOptionObject(SelectionOptionObject selectionOptionObject)
    {
        _selectionOptionObject = selectionOptionObject;
        
        _pointFollowerUI.SetTarget(selectionOptionObject.transform);
        SetEffectName(_selectionOptionObject.OptionName);
        SetEffectDescription(_selectionOptionObject.OptionDescription);
        _effectIcon.sprite = _selectionOptionObject.Icon;
        
        await UniTask.WaitForFixedUpdate();
        
        UpdateContentSizeFilters();
    }

    private void SetEffectName(string effectName)
    {
        _nameTextField.text = effectName;
    }
    
    private void SetEffectDescription(string effectDescription)
    {
        _descriptionTextField.text = _tooltipTextParser.ParseTooltipText(effectDescription, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _inspectionSubpanelsController.SetTooltipParser(_tooltipDataParser.GetTooltipTagDataFromText(_selectionOptionObject.OptionDescription));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        _inspectionSubpanelsController.ClearAllSubpanels();
    }    
    
    private void UpdateContentSizeFilters()
    {
        _contentSizeFitters.ForEach(contentSizeFitter =>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentSizeFitter.transform as RectTransform);
        });
    }
}