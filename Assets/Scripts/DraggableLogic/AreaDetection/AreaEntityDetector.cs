using UnityEngine;

namespace Combat
{
    public class AreaEntityDetector : AreaDetector<CombatEntity>
    {
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out CombatEntity entity))
            {
                RemoveEntity(entity);
            }
        }
        
        private void RemoveEntity(CombatEntity entity)
        {
            if (entity.ComponentsContainer.Has<EntityHealth>())
            {
                entity.ComponentsContainer.Get<EntityHealth>().EntityDied -= RemoveEntity;
            }
            
            if (entity.ComponentsContainer.Has<DraggableEntity>()) 
            { 
                DraggableEntity draggable = entity.ComponentsContainer.Get<DraggableEntity>();
                
                draggable.EntityPlaced -= AddPlacedEntity; 
                draggable.EntityPickedUp -= RemovePickedUpEntity;
            }
            
            RemoveItem(entity);
        }
        
        private void OnTriggerEnter(Collider other)
        { 
            if (other.gameObject.TryGetComponent(out CombatEntity entity))
            {
                OnEntityFound(entity);
            }
        }
        
        private void OnEntityFound(CombatEntity entity)
        {
            if (entity.ComponentsContainer.Has<EntityHealth>())
            {
                entity.ComponentsContainer.Get<EntityHealth>().EntityDied += RemoveEntity;
            }
            
            if (entity.ComponentsContainer.Has<DraggableEntity>())
            {
                DraggableEntity draggable = entity.ComponentsContainer.Get<DraggableEntity>();
                
                draggable.EntityPlaced += AddPlacedEntity; 
                draggable.EntityPickedUp += RemovePickedUpEntity;
                
                if (draggable.IsPlaced == false) return;
            }
            
            AddItem(entity);
        }
        
        private void AddPlacedEntity(CombatEntity entity)
        {
            AddItem(entity);
        }
    
        private void RemovePickedUpEntity(CombatEntity entity)
        {
            RemoveItem(entity);
        }
    }
}