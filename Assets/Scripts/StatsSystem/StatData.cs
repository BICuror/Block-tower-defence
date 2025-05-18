using UnityEngine;
using System;

[CreateAssetMenu(fileName = "StatSystem/StatData", menuName = "StatData")]

public sealed class StatData : ScriptableObject 
{
    [SerializeField] private string _statType;
    
    public Type GetStatType() => Type.GetType(_statType);

    private void OnValidate()
    {
        Type statType = Type.GetType(_statType);

        if (statType == null)
        {
            Debug.LogError($"Wrong stat name {_statType}");
        }
    }
}