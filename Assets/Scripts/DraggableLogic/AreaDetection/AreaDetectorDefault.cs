using UnityEngine;

namespace Combat
{
    public abstract class AreaDetectorDefault<T> : AreaDetector<T> where T: Component
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out T component))
            {
                if (other.gameObject.TryGetComponent<DraggableObject>(out DraggableObject draggable))
                {
                    Debug.Log("Added DraggableObject");
                    draggable.DraggablePlaced += AddPlacedDraggable;
                    draggable.DraggablePickedUp += RemovePickedUpDraggable;
                    
                    if (!draggable.IsPlaced)
                    {
                        return;
                    }
                }
                AddItem(component);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out T component))
            {
                if (other.gameObject.TryGetComponent<DraggableObject>(out DraggableObject draggable))
                {
                    Debug.Log("Removed DraggableObject");
                    draggable.DraggablePlaced -= AddPlacedDraggable;
                    draggable.DraggablePickedUp -= RemovePickedUpDraggable;
                }
                
                RemoveItem(component);
            }
        }
        private void AddPlacedDraggable(DraggableObject draggable)
        { 
            AddItem(draggable.GetComponent<T>());
        }
        
        private void RemovePickedUpDraggable(DraggableObject draggable) 
        {
            RemoveItem(draggable.GetComponent<T>());
        }
    }
}