using Cashing;
using System;

namespace Combat
{
    public class BuildingHealth : EntityHealth
    {
        [Cached] BuildingDraggable _buildingDraggable;
        [Cached] BuildingEntity _ownerEntity;

        public event Action<BuildingEntity> BuildingRevived;
        public event Action<BuildingEntity> BuildingDestroyed;

        public void ReviveBuilding()
        {
            _ownerEntity.gameObject.SetActive(true);
            ReceivePercentHeal(0.1f);
            BuildingRevived?.Invoke(_ownerEntity);
        }
        
        protected override void InvokeEntityDied()
        {
            BuildingDestroyed?.Invoke(_ownerEntity);
        }
    }
}