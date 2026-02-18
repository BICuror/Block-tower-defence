using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using CuroLocalization;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public sealed class CustomDropdown : MonoBehaviour
{
    [SerializeField] private ScrollMaxHeightController _scrollMaxHeightController;
    [SerializeField] private CustomDropdownItem _customDropdownItemPrefab;
    [SerializeField] private TextMeshProUGUI _selectedItemLabel;
    [SerializeField] private CanvasGroup _dropdownGroup;
    [SerializeField] private Transform _itemParent;
    [SerializeField] private Button _button;
    
    [Header("Localiztion")]
    [SerializeField] private List<CustomDropdownItemLocalizationContainer> _localizationContainers;
    private List<CustomDropdownItem> _instantiatedItems = new();
    private int _selectedValue;

    public Action<int> SelectedValueUpdated;

    private void Awake()
    {
        _button.onClick.AddListener(ToggleDropdown);

        LocalizationManager.OnLanguageChanged += UpdateLocalization;
    }

    private void OnDisable() => _dropdownGroup.gameObject.SetActive(false);

    private void ToggleDropdown()
    {
        if (!_dropdownGroup.gameObject.activeSelf)
        {
            _dropdownGroup.gameObject.SetActive(true);
            _scrollMaxHeightController.UpdateHeight().Forget();
        }
        else
        {
            _dropdownGroup.gameObject.SetActive(false);
        }
    }

    private void DisableDropdown()
    {
        _dropdownGroup.gameObject.SetActive(false);
    }

    public void SetItemValues(List<int> indexes)
    {
        _instantiatedItems.ForEach(item => Destroy(item.gameObject));
        _instantiatedItems.Clear();
        indexes.ForEach(CreateCustomDropdownItem);
    }
    
    public void SelectItem(int itemValue)
    {
        _selectedValue = itemValue;

        DisableDropdown();
        SetSelectedItem(itemValue);
        
        SelectedValueUpdated?.Invoke(itemValue);
    }

    public void SetSelectedItem(int itemValue)
    {
        _instantiatedItems.ForEach(item => item.SetSelectedState(item.Value == itemValue));
        
        _selectedItemLabel.text = _localizationContainers.Find(container => container.ItemValue == itemValue).LocalizationKey.Localize();
    }   
    
    private void CreateCustomDropdownItem(int itemValue)
    {
        CustomDropdownItem item = Instantiate(_customDropdownItemPrefab, _itemParent);
        _instantiatedItems.Add(item);
        
        item.Initialize(itemValue, _localizationContainers.Find(container => container.ItemValue == itemValue).LocalizationKey, SelectItem);
    }

    private void UpdateLocalization()
    {
        _selectedItemLabel.text = _localizationContainers.Find(container => container.ItemValue == _selectedValue).LocalizationKey.Localize();
        _instantiatedItems.ForEach(item => item.UpdateLocalization());
    }

    private void OnDestroy()
    {
        LocalizationManager.OnLanguageChanged += UpdateLocalization;
    }
}

[Serializable] public record CustomDropdownItemLocalizationContainer
{
    public int ItemValue;
    public string LocalizationKey;
}