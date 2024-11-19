using Cashing;
using System;

namespace Combat
{
    public abstract class DraggableEntity : DraggableObject
    {
        [Cached] private CombatEntity _ownerEntity;
        
        public Action<CombatEntity> EntityPickedUp;
        public Action<CombatEntity> EntityPlaced;

        private void Awake()
        {
            PickedUp += OnEntityPickedUp;
            Placed += OnEntityPlaced;
        }
        
        private void OnEntityPickedUp() => EntityPickedUp?.Invoke(_ownerEntity);
        private void OnEntityPlaced() => EntityPlaced?.Invoke(_ownerEntity);

        protected void OnDestroy()
        {
            PickedUp -= OnEntityPickedUp;
            Placed -= OnEntityPlaced;
        }
    }
}