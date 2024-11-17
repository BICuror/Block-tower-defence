using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public abstract class AreaDetectorWithHealthSubscription<T> : AreaDetector<T> where T: MonoBehaviour
    {
        private List<EntityHealth> _itemsHealth = new List<EntityHealth>();
    
        private void Awake()
        {   
            AddedItem += OnItemAdded;
            RemovedItem += OnItemRemoved;
        }
    
        private void OnItemAdded(T other)
        {
            EntityHealth entityHealth = other.gameObject.GetComponent<EntityHealth>();
    
            _itemsHealth.Add(entityHealth);
    
            entityHealth.EntityDied += RemoveDestroyedComponent;
        }

        private void OnItemRemoved(T other)
        {
            EntityHealth entityHealth = other.gameObject.GetComponent<EntityHealth>();

            _itemsHealth.Remove(entityHealth);

            entityHealth.EntityDied -= RemoveDestroyedComponent;
        }

        private void RemoveDestroyedComponent(CombatEntity entity) => RemoveComponent(entity.GetComponent<T>());
    }
}

