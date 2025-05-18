using Cashing;
using System;

namespace Combat
{
    public class DraggableEntity : DraggableObject
    {
        [Cached] private CombatEntity _ownerEntity;
        
        public Action<CombatEntity> EntityPickedUp;
        public Action<CombatEntity> EntityPlaced;

        protected void Start()
        {
            PickedUp += OnEntityPickedUp;
            Placed += OnEntityPlaced;
        }

        private void OnEntityPickedUp()
        {
            _ownerEntity.DamageModifierContainer.ReciverContainer.Add<InvincibilityDamageModifier>();
            EntityPickedUp?.Invoke(_ownerEntity);
        }

        private void OnEntityPlaced()
        {
            _ownerEntity.DamageModifierContainer.ReciverContainer.Remove<InvincibilityDamageModifier>();
            EntityPlaced?.Invoke(_ownerEntity);
        }

        protected void OnDestroy()
        {
            PickedUp -= OnEntityPickedUp;
            Placed -= OnEntityPlaced;
        }
    }
}