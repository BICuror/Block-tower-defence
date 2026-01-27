using UnityEngine;
using Cashing;

namespace Combat
{
    public class AreaEntityDetector : AreaDetector<CombatEntity>
    {
        [Cached] private CombatEntity _ownerEntity;
        private EntityDetectorPriorityAlgorithm _priorityAlgorithm;

        private void Start()
        {
            if (_ownerEntity) _ownerEntity.Health.Died += RemoveAll;
        }
        
        public void SetPriorityAlgorithm(EntityDetectorPriorityAlgorithm priorityAlgorithm) => _priorityAlgorithm = priorityAlgorithm;
        
        public CombatEntity GetPrioritizedEntity()
        {
            if (_priorityAlgorithm == null) return List[0];

            return _priorityAlgorithm.GetPrioritizedEntity(List);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out CombatEntity entity))
            {
                if (ContainsItem(entity)) RemoveEntity(entity);
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
                if (!ContainsItem(entity)) OnEntityFound(entity);
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

        private void OnDestroy()
        {
            if (_ownerEntity) _ownerEntity.Health.Died -= RemoveAll;
        }
    }
}