using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Combat
{
    public abstract class AreaDetector <T> : MonoBehaviour where T: Component 
    {
        protected List<T> List = new();
    
        public Action<T> AddedItem;
        public Action<T> RemovedItem;
        
        public bool IsEmpty => List.Count == 0;
        public T RandomItem => List[Random.Range(0, List.Count)];
        public T FirstItem => List[0];
        
        public IReadOnlyList<T> GetList() => List;
        
        protected void AddItem(T component)
        {
            List.Add(component);
            AddedItem?.Invoke(component);
        }
        
        protected void RemoveItem(T component)
        { 
            List.Remove(component);
            RemovedItem?.Invoke(component);
        }
        
        private void RemoveAll()
        {
            while (List.Count > 0)
            {
                if (List[^1] != null) RemoveItem(List[^1]);
                else List.RemoveAt(List.Count - 1);
            }

            List.Clear();
        }
        
        private void OnDestroy() => RemoveAll();
    }
}