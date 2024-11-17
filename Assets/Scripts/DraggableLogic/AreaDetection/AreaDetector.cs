using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Combat
{
    public abstract class AreaDetector <T> : AreaDetectorBase where T: MonoBehaviour 
    {
        protected List<T> List = new List<T>();
    
        public Action<T> AddedItem;
        public Action<T> RemovedItem;
        
        public bool IsEmpty() => List.Count == 0;
        public T GetRandomItem() => List[Random.Range(0, List.Count)];
        public T GetFirstItem() => List[0];
        public IReadOnlyList<T> GetList() => List;
    
        public void ClearList() => List = new List<T>();
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out T component))
            {
                AddComponent(component);
            }
        }
        protected void AddComponent(T component)
        {
            List.Add(component);
    
            AddedItem?.Invoke(component);
        }
    
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out T component))
            {
                RemoveComponent(component);
            }
        }
        protected void RemoveComponent(T component)
        {
            List.Remove(component);
    
            RemovedItem?.Invoke(component);
        }
    
        private void RemoveAll()
        {
            List.ForEach(item =>
            {
                if (item != null) RemoveComponent(item);
            });
            
            List.Clear();
        }
        
        private void OnDisable() => RemoveAll();
        private void OnDestroy() => RemoveAll();
    }
}