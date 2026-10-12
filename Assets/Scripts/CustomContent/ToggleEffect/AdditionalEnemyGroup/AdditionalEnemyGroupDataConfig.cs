using System.Collections.Generic;
using UnityEngine;
using System;

namespace Combat.EnemySpawnSystem
{
    [Serializable] public sealed class AdditionalEnemyGroupDataConfig
    {
        [Range(0f, 1f)] [SerializeField] private float _amountMultiplier;
        [SerializeField] private List<EnemyGroupPart> _groupParts;
        
        public float AmountMultiplier => _amountMultiplier;
        public List<EnemyGroupPart> GroupParts => _groupParts;
    }
}