using System;
using Cashing;

namespace Combat
{
    public class BuildingHealth : EntityHealth
    {
        [Cached] BuildingDraggable _buildingDraggable;
        [Cached] BuildingEntity _ownerEntity;
        
        public Action<BuildingEntity> BuildingDestroyed;
    
        public override void Die()
        {
            base.Die();
            BuildingDestroyed.Invoke(_ownerEntity);
            Destroy(gameObject);
        }
    }
}