using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public sealed class SettingsScreenTabButton : MonoBehaviour
{
    [SerializeField] private List<CanvasGroup> _selectedStateCanvasGroups;
    [SerializeField] private Button _button;
    
    public Action<SettingsScreenTabButton> Selected;

    private void Awake()
    {
        _button.onClick.AddListener(Select);
    }

    public void SetState(bool state)
    {
        _selectedStateCanvasGroups.ForEach(stateCanvasGroup => stateCanvasGroup.gameObject.SetActive(state));
    }

    private void Select() => Selected?.Invoke(this);
}