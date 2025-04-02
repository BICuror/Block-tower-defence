using UnityEngine;

namespace Combat
{
    public class AreaEntityDetector<T> : AreaDetector<T> where T : Component
    {
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out CombatEntity entity))
            {
                if (!entity.ComponentsContainer.Has<T>()) return;
                
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
            
            RemoveItem(entity.ComponentsContainer.Get<T>());
        }
        
        private void OnTriggerEnter(Collider other)
        { 
            if (other.gameObject.TryGetComponent(out CombatEntity entity))
            {
                if (!entity.ComponentsContainer.Has<T>()) return;
                
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
            
            AddItem(entity.ComponentsContainer.Get<T>());
        }
        
        private void AddPlacedEntity(CombatEntity entity)
        {
            AddItem(entity.ComponentsContainer.Get<T>());
        }
    
        private void RemovePickedUpEntity(CombatEntity entity)
        {
            RemoveItem(entity.ComponentsContainer.Get<T>());
        }
    }
}