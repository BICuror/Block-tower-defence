using Cashing;
using System;

namespace Combat
{
    public class BuildingHealth : EntityHealth
    {
        [Cached] BuildingDraggable _buildingDraggable;
        [Cached] BuildingEntity _ownerEntity;

        public event Action<BuildingEntity> BuildingDestroyed;
        
        protected override void InvokeEntityDied()
        {
            BuildingDestroyed?.Invoke(_ownerEntity);
        }
    }
}