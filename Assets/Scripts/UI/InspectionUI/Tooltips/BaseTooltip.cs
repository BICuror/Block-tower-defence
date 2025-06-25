using UnityEngine.EventSystems;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public abstract class BaseTooltip : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] private List<ContentSizeFitter> _contentSizeFitters;
    [SerializeField] private List<CanvasGroup> _selectedState;
    [SerializeField] private CanvasGroup _amountGroup;
    [SerializeField] private TextMeshProUGUI _amountTextField;
    private int _itemAmount = 1;
    
    protected abstract TooltipParseTagDataContainer TagDataContainer {get;}
    
    public Action<TooltipParseTagDataContainer> TooltipOpened;
    public Action TooltipClosed;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _selectedState.ForEach(group => group.gameObject.SetActive(true));
        TooltipOpened.Invoke(TagDataContainer);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        _selectedState.ForEach(group => group.gameObject.SetActive(false));
        TooltipClosed.Invoke();
    }

    public void IncreaseAmount()
    {
        _itemAmount++;
        
        _amountTextField.gameObject.SetActive(true);
        _amountTextField.text = _itemAmount.ToString();
        
        GetComponent<RectTransform>().ForceUpdateRectTransforms();
    }
    
    public void SetAmount(int amount)
    {
        if (amount == 1) return;
        
        _itemAmount = amount;
        
        _amountGroup.gameObject.SetActive(true);
        _amountTextField.text = _itemAmount.ToString();
        
        GetComponent<RectTransform>().ForceUpdateRectTransforms();
    }

    public void UpdateContentSizeFilters()
    {
        _contentSizeFitters.ForEach(contentSizeFitter =>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentSizeFitter.transform as RectTransform);
        });
    }
}