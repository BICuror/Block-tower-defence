using UnityEngine;
using Cashing;

namespace Combat
{
    [RequireComponent(typeof(StatContainer))]
    
    public abstract class CombatEntity : EntityComponentCacher
    {
        [Cached] private BoxCollider boxCollider;
        [Cached] private EntityHealth _entityHealth;
        private EntityEffectManager _entityEffectManager;

        public CachedComponentsContainer ComponentsContainer => CachedComponentsContainer;
        public IHealth Health => CachedComponentsContainer.Get<EntityHealth>();
        public EntityEffectManager EntityEffectManager => CachedComponentsContainer.Get<EntityEffectManager>();
    }
}
