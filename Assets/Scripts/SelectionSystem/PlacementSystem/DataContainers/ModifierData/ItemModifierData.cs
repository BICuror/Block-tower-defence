using System;
using UnityEngine;

public abstract class ItemModifierData : ScriptableObject
{
    [Range(-5, 5)] [SerializeField] private int _quality = 3;
    [SerializeField] private string _modifierTypeName;
    [SerializeField] private string _apperanceConditionTypeName;
    [SerializeField] private bool _isUnique = false;
    
    public Type Type => Type.GetType(_modifierTypeName);
    public bool HasApperanceCondition => _apperanceConditionTypeName != "";
    public Type ApperanceConditionTypeName => Type.GetType(_apperanceConditionTypeName);
    public int Quality => _quality;
    public bool IsUnique => _isUnique;
    
    private void OnValidate()
    {
        if (Type == null)
        {
            Debug.LogError("Invalid modifier type");
        }

        if (HasApperanceCondition && ApperanceConditionTypeName == null)
        {
            Debug.LogError("Invalid apperance condition type");
        }
    }
}