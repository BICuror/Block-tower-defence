using UnityEngine;
using Cashing;

namespace Combat
{
    [RequireComponent(typeof(StatContainer))]
    
    public abstract class CombatEntity : EntityComponentCacher
    {
        private EntityDamageModifierContainer _damageModifierContainer = new();
        [Cached] private EntityHealth _entityHealth;
        [Cached] private StatContainer _statContainer;

        public CachedComponentsContainer ComponentsContainer => CachedComponentsContainer;
        public StatContainer StatContainer => _statContainer;
        public EntityDamageModifierContainer DamageModifierContainer => _damageModifierContainer;
        public DraggableObject Draggable => CachedComponentsContainer.Get<DraggableObject>();
        public abstract EntityHealth Health { get; }
    }
}
