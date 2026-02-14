using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Combat
{
    public abstract class AreaDetector <T> : MonoBehaviour where T : Component 
    {
        protected List<T> List = new();
        public bool IsEmpty => List.Count == 0;
        public T RandomItem => List[Random.Range(0, List.Count)];
        public int Count => List.Count;
        
        public event Action<T> AddedItem;
        public event Action<T> RemovedItem;

        public IReadOnlyList<T> GetList() => List;
        
        protected bool ContainsItem(T item) => List.Contains(item);
        
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
        
        protected void RemoveAll()
        {
            for (int i = 0; i < List.Count; i++)
            {
                RemovedItem?.Invoke(List[i]);
            }

            List.Clear();
        }
    }
}