using System.Collections.Generic;
using UnityEngine.UI;
using CuroSettings;
using UnityEngine;

public sealed class SettingsScreen : MonoBehaviour
{
    [SerializeField] private List<SettingsScreenTabButton> _settingsScreenTabButtons;
    [SerializeField] private Button _revertButton;
    [SerializeField] private Button _applyButton;

    private void Awake()
    {
        _settingsScreenTabButtons.ForEach(tabButton => tabButton.Selected += SelectTab);
        _revertButton.onClick.AddListener(SettingsContainer.LoadAll);
        _applyButton.onClick.AddListener(SettingsContainer.SaveAll);
        
        SelectTab(_settingsScreenTabButtons[0]);
    }

    private void SelectTab(SettingsScreenTabButton selectedTabButton)
    {
        _settingsScreenTabButtons.ForEach(tabButton => tabButton.SetState(selectedTabButton == tabButton));
    }

    private void OnDisable()
    {
        SettingsContainer.SaveAll();
    }
}