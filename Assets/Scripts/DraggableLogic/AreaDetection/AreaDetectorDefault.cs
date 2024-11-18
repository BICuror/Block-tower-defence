using UnityEngine;

namespace Combat
{
    public abstract class AreaDetectorDefault<T> : AreaDetector<T> where T: Component
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out T component))
            {
                AddItem(component);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out T component))
            {
                RemoveItem(component);
            }
        }
    }
}