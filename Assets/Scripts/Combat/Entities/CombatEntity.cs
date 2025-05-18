using Cashing;

namespace Combat
{
    public abstract class CombatEntity : EntityComponentCacher
    {
        private EntityDamageModifierContainer _damageModifierContainer;

        public EntityDamageModifierContainer DamageModifierContainer => _damageModifierContainer;
        public DraggableObject Draggable => ComponentsContainer.Get<DraggableObject>();
        public EntityHealth Health => ComponentsContainer.Get<EntityHealth>();

        protected void Awake()
        {
            base.Awake();
            
            _damageModifierContainer = new EntityDamageModifierContainer(this);
        }
    }
}
