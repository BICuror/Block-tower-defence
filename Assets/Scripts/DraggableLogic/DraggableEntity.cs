using Cashing;
using System;

namespace Combat
{
    public class DraggableEntity : DraggableObject
    {
        [Cached] protected CombatEntity OwnerEntity;
        
        public Action<CombatEntity> EntityPickedUp;
        public Action<CombatEntity> EntityPlaced;

        protected void Start()
        {
            PickedUp += OnEntityPickedUp;
            Placed += OnEntityPlaced;
        }

        private void OnEntityPickedUp()
        {
            OwnerEntity.Health.InvulnerabilityTokenContainer.AddToken();
            EntityPickedUp?.Invoke(OwnerEntity);
        }

        private void OnEntityPlaced()
        {
            OwnerEntity.Health.InvulnerabilityTokenContainer.RemoveToken();
            EntityPlaced?.Invoke(OwnerEntity);
        }

        protected void OnDestroy()
        {
            PickedUp -= OnEntityPickedUp;
            Placed -= OnEntityPlaced;
        }
    }
}