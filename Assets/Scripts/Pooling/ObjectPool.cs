using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;
using Zenject;
using System;

public sealed class ObjectPool<T> where T : Component
{
    private List<T> _pool;
    private int _pointer;
    private T _prefab;
    private Transform _container;
    
    private DiContainer _diContainer;
    private Predicate<T> _isFreeElement;
    
    public Action<T> ObjectCreated;

    public IReadOnlyList<T> Pool => _pool;
    
    private int ActiveCount => _pool.Count(pooledObject => pooledObject && pooledObject.gameObject.activeSelf);
    
    public ObjectPool(T prefab, int poolSize, DiContainer diContainer = null, Predicate<T> isFreeElement = null)
    {
        _prefab = prefab;
        _container = new GameObject().transform;
        
        _diContainer = diContainer;
        _isFreeElement = isFreeElement;
        
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
        if (HasFreeElement(out T element))
        {
            element.gameObject.SetActive(true);

            return element;
        }

        return CreatePooledObject();
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
            MovePointer();
            
            if (!_pool[_pointer].gameObject.activeSelf)
            {
                if (_isFreeElement == null || _isFreeElement.Invoke(_pool[_pointer]))
                {
                    element = _pool[_pointer];

                    return true;
                }
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

    public async void DestroyPool()
    {
        await UniTask.WaitUntil(() => ActiveCount == 0);
        
        for (int i = 0; i < _pool.Count; i++)
        {
            if (_pool[i] != null) MonoBehaviour.Destroy(_pool[i].gameObject);
        }

        if (_container) MonoBehaviour.Destroy(_container.gameObject);
    }
    
    private T CreatePooledObject()
    {
        T pooledObject = MonoBehaviour.Instantiate(_prefab, Vector3.zero, Quaternion.identity, _container);
        pooledObject.gameObject.SetActive(true);
        _pool.Add(pooledObject);
        
        if (_diContainer != null) _diContainer.Inject(pooledObject);
        
        ObjectCreated?.Invoke(pooledObject);
        
        return pooledObject;
    }
}