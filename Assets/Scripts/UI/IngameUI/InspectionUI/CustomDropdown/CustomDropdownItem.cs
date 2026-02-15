using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public sealed class CustomDropdownItem : MonoBehaviour
{
    [Header("RequiredReferences")]
    [SerializeField] private TextMeshProUGUI _selectedItemLabel;
    [SerializeField] private Button _button;
 
    [Header("SelectionIndicator")]
    [SerializeField] private CanvasGroup _selectionIndicator;
    private Action<int> _onClickAction;
    private int _itemValue;
    
    public int Value => _itemValue;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }
    
    public void Initialize(int itemValue, string itemLabel, Action<int> onClickAction)
    {
        _itemValue = itemValue;
        _selectedItemLabel.text = itemLabel;
        _onClickAction = onClickAction;
    }
    
    public void SetSelectedState(bool selected) => _selectionIndicator.gameObject.SetActive(selected);
    
    private void OnClick() => _onClickAction.Invoke(_itemValue);
}