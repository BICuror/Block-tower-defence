using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public sealed class EnemyEffectStand : MonoBehaviour
    {
        [SerializeField] private Effect[] _applyEffects;
    
        [SerializeField] private EnemyEffectManagerAreaScaner _enemyEffectManagerAreaScaner;
    
        private void Start()
        {
            _enemyEffectManagerAreaScaner.AddedItem += AddEffectToEffectManager;
            _enemyEffectManagerAreaScaner.RemovedItem += RemoveEffectFromEffectManager;
        }
    
        private void AddEffectToEffectManager(EnemyEffectManager enemyEffectManager)
        {
            for (int i = 0; i < _applyEffects.Length; i++)
            {
                enemyEffectManager.ApplyEffect(_applyEffects[i]);
            }
        }
        
        private void RemoveEffectFromEffectManager(EnemyEffectManager enemyEffectManager)
        {
            for (int i = 0; i < _applyEffects.Length; i++)
            {
                enemyEffectManager.TryToRemoveEffect(_applyEffects[i]);
            }
        }
    }
}