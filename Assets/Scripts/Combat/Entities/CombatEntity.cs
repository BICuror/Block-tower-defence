using UnityEngine;
using Cashing;

namespace Combat
{
    [RequireComponent(typeof(StatContainer))]
    
    public abstract class CombatEntity : EntityComponentCacher
    {
        [Cached] public BoxCollider boxCollider;
        
        [Cached] private EntityHealth _entityHealth;
        private EntityEffectManager _entityEffectManager;
    
        public IHealth Health => CachedComponentsContainer.Get<EntityHealth>();
        public EntityEffectManager EntityEffectManager => CachedComponentsContainer.Get<EntityEffectManager>();
    }
}
