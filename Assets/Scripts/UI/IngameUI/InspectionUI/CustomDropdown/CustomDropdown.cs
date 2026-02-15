using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using TMPro;

public sealed class CustomDropdown : MonoBehaviour
{
    [SerializeField] private ScrollMaxHeightController _scrollMaxHeightController;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _selectedItemLabel;
    [SerializeField] private CustomDropdownItem _customDropdownItemPrefab;
    [SerializeField] private Transform _itemParent;
    [SerializeField] private CanvasGroup _dropdownGroup;
    
    private List<CustomDropdownItemData> _customDropdownDatas;
    private List<CustomDropdownItem> _instantiatedItems = new();

    public Action<int> SelectedValueUpdated;

    private void Awake()
    {
        _button.onClick.AddListener(EnableDropdownAsync);
    }

    private void EnableDropdownAsync() => EnableDropdown().Forget();
    private async UniTask EnableDropdown()
    {
        _dropdownGroup.gameObject.SetActive(true);
        await UniTask.WaitForFixedUpdate();
        _scrollMaxHeightController.UpdateHeight().Forget();
    }

    private void DisableDropdown()
    {
        _dropdownGroup.gameObject.SetActive(false);
    }

    public void SetItemDatas(List<CustomDropdownItemData> datas)
    {
        _customDropdownDatas = datas;
        
        _instantiatedItems.ForEach(item => Destroy(item.gameObject));
        _instantiatedItems.Clear();
        datas.ForEach(CreateCustomDropdownItem);
    }
    
    public void SelectItem(int itemValue)
    {
        _instantiatedItems.ForEach(item => item.SetSelectedState(item.Value == itemValue));
        
        SelectedValueUpdated?.Invoke(itemValue);
        _selectedItemLabel.text = _customDropdownDatas.Find(data => data.Value == itemValue).Text;
        
        DisableDropdown();
    }
    
    private void CreateCustomDropdownItem(CustomDropdownItemData data)
    {
        CustomDropdownItem item = Instantiate(_customDropdownItemPrefab, _itemParent);
        _instantiatedItems.Add(item);
        
        item.Initialize(data.Value, data.Text, SelectItem);
    }
}

public record CustomDropdownItemData
{
    public int Value;
    public string Text;
    
    public CustomDropdownItemData(int itemValue, string itemText)
    {
        Value = itemValue;
        Text = itemText;
    }
}