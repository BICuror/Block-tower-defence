using Cashing;
using System;

namespace Combat
{
    public class DraggableEntity : DraggableObject
    {
        [Cached] private CombatEntity _ownerEntity;
        
        public Action<CombatEntity> EntityPickedUp;
        public Action<CombatEntity> EntityPlaced;

        private void Start()
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