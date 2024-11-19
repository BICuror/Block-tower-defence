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

                if (draggable.IsPlaced() == false)
                {
                    draggable.EntityPlaced += AddPlacedEntity;
                }
            }
            
            AddItem(entity.ComponentsContainer.Get<T>());
        }
        
        private void AddPlacedEntity(CombatEntity entity)
        {
            DraggableEntity draggable = entity.ComponentsContainer.Get<DraggableEntity>();
            
            draggable.EntityPlaced -= AddPlacedEntity;
            draggable.EntityPickedUp += RemovePickedUpEntity;

            AddItem(entity.ComponentsContainer.Get<T>());
        }
    
        private void RemovePickedUpEntity(CombatEntity entity)
        {
            DraggableEntity draggable = entity.ComponentsContainer.Get<DraggableEntity>();
            
            draggable.EntityPickedUp -= RemovePickedUpEntity;
            draggable.EntityPlaced += AddPlacedEntity;
            
            RemoveItem(entity.ComponentsContainer.Get<T>());
        }
    }
}