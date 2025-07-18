using System.Collections.Generic;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public sealed class EffectInspectionTooltipPreview : InspectionPanel, IPointerEnterHandler
{
    [SerializeField] private List<ContentSizeFitter> _contentSizeFitters;
    [SerializeField] private TextMeshProUGUI _previewName;
    [SerializeField] private Image _previewImage;

    public Action PointerEntered;
    
    public async UniTask SetEffectPreview(Sprite sprite, string nameToDisplay)
    {
        _previewName.text = nameToDisplay;
        _previewImage.sprite = sprite;

        await UniTask.WaitForFixedUpdate();
        
        UpdateContentSizeFilters();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEntered?.Invoke();
    }
    
    private void UpdateContentSizeFilters()
    {
        _contentSizeFitters.ForEach(contentSizeFitter =>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentSizeFitter.transform as RectTransform);
        });
    }
}