using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] public sealed class AdditionalEnemyGroupData
{
    [Range(0f, 1f)] [SerializeField] private float _amountMultiplier;
    [SerializeField] private List<EnemyWaveGroup.GroupPart> _groupParts;
    
    public float AmountMultiplier => _amountMultiplier;
    public List<EnemyWaveGroup.GroupPart> GroupParts => _groupParts;
}