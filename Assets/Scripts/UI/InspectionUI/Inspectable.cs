using UnityEngine;
using System;

public sealed class Inspectable : MonoBehaviour
{
    [SerializeField] private bool _canBeIdleInspected;
    [SerializeField] private string _inspectableObjectName;
    [SerializeField] private string _inspectableObjectDescription;
    private bool _isInspected;
    
    public string Name => _inspectableObjectName;
    public string Description => _inspectableObjectDescription;
    public bool IsInspected => _isInspected;
    public bool CanBeIdleInspected => _canBeIdleInspected;

    public Action InspectionStarted;
    public Action InspectionEnded;
    
    public void SetInspectedState(bool state)
    {
        _isInspected = state;
        
        if (_isInspected) InspectionStarted?.Invoke();
        else InspectionEnded?.Invoke();
    }
}