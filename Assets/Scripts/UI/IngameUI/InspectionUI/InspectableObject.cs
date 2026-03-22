using CuroLocalization;
using UnityEngine;
using System;

public sealed class InspectableObject : MonoBehaviour
{
    [SerializeField] private bool _pauseOnInspection;
    [SerializeField] private bool _canBeIdleInspected;
    [SerializeField] private string _localizationKey;
    private ReplaceableDataParser _replaceableDataParser = new();
    private bool _isInspected;
    
    public bool IsInspected => _isInspected;
    public bool CanBeIdleInspected => _canBeIdleInspected;
    
    public bool PauseOnInspection => _pauseOnInspection;
    public string Name => (_localizationKey + "_header").Localize();
    public string Description => "startTag " + (_localizationKey + "_description").Localize();
    public ReplaceableDataParser ReplaceableDataParser => _replaceableDataParser;

    public event Action InspectionStarted;
    public event Action InspectionEnded;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(_localizationKey)) SetLocalizationKey(_localizationKey);
    }
    
    public void SetCanBeIdleInspected(bool state) => _canBeIdleInspected = state;
    
    public void SetInspectedState(bool state)
    {
        _isInspected = state;
        
        if (_isInspected) InspectionStarted?.Invoke();
        else InspectionEnded?.Invoke();
    }

    public void SetLocalizationKey(string localizationKey) => _localizationKey = localizationKey;
}