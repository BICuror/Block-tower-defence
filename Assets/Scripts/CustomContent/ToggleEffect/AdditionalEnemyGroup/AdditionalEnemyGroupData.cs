using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] public sealed class AdditionalEnemyGroupData
{
    [Range(0f, 1f)] [SerializeField] private float _amountMultiplier;
    [SerializeField] private List<EnemyGroupPart> _groupParts;
    
    public float AmountMultiplier => _amountMultiplier;
    public List<EnemyGroupPart> GroupParts => _groupParts;
}