using UnityEngine.EventSystems;
using UnityEngine;
using System;
using TMPro;

public abstract class BaseTooltip : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] private TextMeshProUGUI _amountTextField;
    private int _itemAmount = 1;
    
    protected abstract TooltipParseTagDataContainer TagDataContainer {get;}
    
    public Action<TooltipParseTagDataContainer> TooltipOpened;
    public Action TooltipClosed;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipOpened.Invoke(TagDataContainer);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipClosed.Invoke();
    }

    public void IncreaseAmount()
    {
        _itemAmount++;
        
        _amountTextField.gameObject.SetActive(true);
        _amountTextField.text = $"x{_itemAmount}";
    }
    
    public void SetAmount(int amount)
    {
        if (amount == 1) return;
        
        _itemAmount = amount;
        
        _amountTextField.gameObject.SetActive(true);
        _amountTextField.text = $"x{_itemAmount}";
    }
}