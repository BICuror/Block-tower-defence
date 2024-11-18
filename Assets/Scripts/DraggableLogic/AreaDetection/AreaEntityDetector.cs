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
            
            if (entity.ComponentsContainer.Has<DraggableObject>()) 
            { 
                DraggableObject draggableObject = entity.ComponentsContainer.Get<DraggableObject>();
                
                draggableObject.DraggablePlaced -= AddPlacedEntity; 
                draggableObject.DraggablePickedUp -= RemovePickedUpEntity;
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
            
            if (entity.ComponentsContainer.Has<DraggableObject>())
            {
                DraggableObject draggableObject = entity.ComponentsContainer.Get<DraggableObject>();

                if (draggableObject.IsPlaced() == false)
                {
                    draggableObject.DraggablePlaced += AddPlacedEntity;
                }
            }
            
            AddItem(entity.ComponentsContainer.Get<T>());
        }
        
        private void AddPlacedEntity(CombatEntity entity)
        {
            DraggableObject draggableObject = entity.ComponentsContainer.Get<DraggableObject>();
            
            draggableObject.DraggablePlaced -= AddPlacedEntity;
            draggableObject.DraggablePickedUp += RemovePickedUpEntity;

            AddItem(entity.ComponentsContainer.Get<T>());
        }
    
        private void RemovePickedUpEntity(CombatEntity entity)
        {
            DraggableObject draggableObject = entity.ComponentsContainer.Get<DraggableObject>();
            
            draggableObject.DraggablePickedUp -= RemovePickedUpEntity;
            draggableObject.DraggablePlaced += AddPlacedEntity;
            
            RemoveItem(entity.ComponentsContainer.Get<T>());
        }
    }
}