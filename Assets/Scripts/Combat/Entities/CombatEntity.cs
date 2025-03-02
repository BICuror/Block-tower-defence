using UnityEngine;
using Cashing;

namespace Combat
{
    [RequireComponent(typeof(StatContainer))]
    
    public abstract class CombatEntity : EntityComponentCacher
    {
        [Cached] private EntityHealth _entityHealth;
        [Cached] private StatContainer _statContainer;
        private EntityEffectManager _entityEffectManager;

        public CachedComponentsContainer ComponentsContainer => CachedComponentsContainer;
        public StatContainer StatContainer => _statContainer;
        public IHealth Health => CachedComponentsContainer.Get<EntityHealth>();
        public EntityEffectManager EntityEffectManager => CachedComponentsContainer.Get<EntityEffectManager>();
    }
}
