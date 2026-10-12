using System.Collections.Generic;
using UnityEngine;
using Configs;

namespace Combat.StatusEffects
{
    [CreateAssetMenu(menuName = "Combat/StatusEffects/StatusEffect")]
    
    public sealed class StatusEffectConfig : UpdatableScriptableObject
    {
        [SerializeField] private DomainConfig _domainConfig;
        
        [Header("RequiredData")] 
        [SerializeField] private bool _isStackable;
        
        public DomainConfig DomainConfig => _domainConfig;
        public bool IsStackable => _isStackable;
        
        protected override IEnumerable<INonScriptableObjectUpdatable> GetAllNonScriptableObjectUpdatables()
        {
            return _domainConfig.GetAllUpdatables();
        }
    }
}