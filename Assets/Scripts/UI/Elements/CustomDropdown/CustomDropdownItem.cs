using UnityEngine.UI;
using UnityEngine;
using System;
using CuroLocalization;
using TMPro;

public sealed class CustomDropdownItem : MonoBehaviour
{
    [Header("RequiredReferences")]
    [SerializeField] private TextMeshProUGUI _selectedItemLabel;
    [SerializeField] private Button _button;
 
    [Header("SelectionIndicator")]
    [SerializeField] private CanvasGroup _selectionIndicator;
    private Action<int> _onClickAction;
    private string _localizationKey;
    private int _itemValue;
    
    public int Value => _itemValue;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }
    
    public void Initialize(int itemValue, string localizationKey, Action<int> onClickAction)
    {
        _itemValue = itemValue;
        _localizationKey = localizationKey;
        _onClickAction = onClickAction;

        UpdateLocalization();
    }
    
    public void SetSelectedState(bool selected) => _selectionIndicator.gameObject.SetActive(selected);
    
    public void UpdateLocalization() => _selectedItemLabel.text = _localizationKey.Localize();
    
    private void OnClick() => _onClickAction.Invoke(_itemValue);
}