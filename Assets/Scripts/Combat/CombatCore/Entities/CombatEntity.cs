using Cashing;
using System;

namespace Combat
{
    public abstract class CombatEntity : EntityComponentCacher, IActivatable
    {
        private EntityDamageModifierContainer _damageModifierContainer;

        public EntityDamageModifierContainer DamageModifierContainer => _damageModifierContainer;
        public DraggableObject Draggable => ComponentsContainer.Get<DraggableObject>();
        public EntityHealth Health => ComponentsContainer.Get<EntityHealth>();

        public Action Activated;

        protected void Awake()
        {
            base.Awake();
            
            _damageModifierContainer = new EntityDamageModifierContainer(this);
        }

        public void Activate() => Activated?.Invoke();
    }
}