using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class ObjectPool<T> where T: Component
{
    private List<T> _pool;
    private int _pointer;
    private T _prefab;
    private Transform _container;
    
    private OnObjectInitialized _onObjectInitialized;
    private DiContainer _diContainer;
    
    public ObjectPool(T prefab, int poolSize, [Optional]DiContainer diContainer, [Optional]OnObjectInitialized onObjectInitialized)
    {
        _prefab = prefab;
        _diContainer = diContainer;
        _onObjectInitialized = onObjectInitialized;
        
        _container = new GameObject().transform;
        
        _container.gameObject.name = GetType().ToString();
        
        InstantiatePool(poolSize);
    }

    private void InstantiatePool(int poolSize)
    {
        _pool = new List<T>();

        for (int i = 0; i < poolSize; i++)
        {   
            T pooledObject = CreatePooledObject();
            
            _pool.Add(pooledObject);
            pooledObject.gameObject.SetActive(false);
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
            if (_pool[i] != null) MonoBehaviour.Destroy(_pool[i].gameObject);
        }

        MonoBehaviour.Destroy(_container.gameObject);
    }
    
    private T CreatePooledObject()
    {
        T pooledObject = MonoBehaviour.Instantiate(_prefab, Vector3.zero, Quaternion.identity, _container);
        _pool.Add(pooledObject);
        
        if (_diContainer != null) _diContainer.Inject(pooledObject);
        if (_onObjectInitialized != null) _onObjectInitialized.Invoke(pooledObject);
        
        return pooledObject;
    }
    
    public delegate void OnObjectInitialized(T pooledObject);
}