using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AreaDetector <T> : AreaDetectorBase where T: MonoBehaviour 
{
    protected List<T> _list = new List<T>();

    public UnityEvent<T> AddedComponent;
    public UnityEvent<T> RemovedComponent;
    
    public bool IsEmpty() => _list.Count == 0;
    public T GetRandomEntity() => _list[Random.Range(0, _list.Count)];
    public T GetFirstEntity() => _list[0];
    public IReadOnlyList<T> GetList() => _list;

    public void ClearList() => _list = new List<T>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out T component))
        {
            AddComponent(component);
        }
    }

    protected void AddComponent(T component)
    {
        _list.Add(component);

        AddedComponent.Invoke(component);
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
        _list.Remove(component);

        RemovedComponent.Invoke(component);
    }

    private void OnDisable() => RemoveAll();
    private void OnDestroy() => RemoveAll();

    private void RemoveAll()
    {
        while(_list.Count > 0)
        {
            if (_list[_list.Count - 1] != null)
            {
                RemoveComponent(_list[_list.Count - 1]);
            }
            else 
            {
                _list.RemoveAt(_list.Count - 1);
            }
        }
    }
}
