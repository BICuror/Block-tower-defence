using CuroLocalization;
using UnityEngine;
using System;

public sealed class Inspectable : MonoBehaviour
{
    [SerializeField] private bool _canBeIdleInspected;

    [Header("UI Elements")] 
    [SerializeField] private string _localizationKey;
    private string _inspectableName;
    private string _inspectableDescription;
    private bool _isInspected;
    
    public bool IsInspected => _isInspected;
    public bool CanBeIdleInspected => _canBeIdleInspected;
    
    public string Name => _inspectableName;
    public string Description => _inspectableDescription;

    public event Action InspectionStarted;
    public event Action InspectionEnded;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(_localizationKey)) SetLocalizationKey(_localizationKey);
    }
    
    public bool SetCanBeIdleInspected(bool state) => _canBeIdleInspected = state;
    
    public void SetInspectedState(bool state)
    {
        _isInspected = state;
        
        if (_isInspected) InspectionStarted?.Invoke();
        else InspectionEnded?.Invoke();
    }

    public void SetLocalizationKey(string localizationKey)
    {
        _localizationKey = localizationKey;
        _inspectableName = (localizationKey + "_header").Localize();
        _inspectableDescription = (localizationKey + "_description").Localize();
    }
    
    public void SetInspectableData(string inspectableName, string inspectableDescription)
    {
        _inspectableName = inspectableName;
        _inspectableDescription = inspectableDescription;
    }
}