using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ObjectPool<T>: MonoBehaviour where T: Component
{   
    private List<T> _pool;

    private int _pointer;

    private Transform _container;

    private T _prefab;

    public ObjectPool(T prefab, int poolSize)
    {
        _container = new GameObject().transform;

        _container.gameObject.name = this.GetType().ToString();

        _prefab = prefab;

        InstantiatePool(poolSize);
    }

    private void InstantiatePool(int poolSize)
    {
        _pool = new List<T>();

        for (int i = 0; i < poolSize; i++)
        {   
            _pool.Add(Object.Instantiate(_prefab, Vector3.zero, Quaternion.identity, _container));

            _pool[i].gameObject.SetActive(false);   
        }
    }

    public T GetNextPooledObject()
    {
        MovePointer();

        if (HasFreeElement(out T element))
        {
            element.gameObject.SetActive(true);

            return element;
        }
        else 
        {
            return CreatePooledObject();
        }
    }   
    
    private void MovePointer()
    {
        _pointer += 1;

        if (_pointer >= _pool.Count) _pointer = 0;
    }

    private bool HasFreeElement(out T element)
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            int currentPointer = _pointer + i;

            if (currentPointer >= _pool.Count) currentPointer -= _pool.Count;

            if (_pool[currentPointer].gameObject.activeSelf == false)
            {
                element = _pool[currentPointer];
            
                return true;
            }
        }

        element = null;

        return false;
    }

    private T CreatePooledObject()
    {
        _pool.Add(Object.Instantiate(_prefab, Vector3.zero, Quaternion.identity, _container));
    
        return _pool[_pool.Count - 1];
    }

    public void DisableAllObjects()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            _pool[i].gameObject.SetActive(false);
        }
    }

    public void DestroyPool()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (_pool[i] != null) Destroy(_pool[i].gameObject);
        }

        Destroy(_container.gameObject);
    }
}
