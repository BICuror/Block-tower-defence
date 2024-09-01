using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AreaDetectorWithHealthSubscription<T> : AreaDetector<T> where T: MonoBehaviour
{
    private List<EntityHealth> _healthComponentsArea = new List<EntityHealth>();

    private UnityEvent<T> RemovedComponentDueToItsDeath;

    public EntityHealth GetFirstEntityHealth() => _healthComponentsArea[0];

    private void Awake()
    {   
        AddedComponent.AddListener(OnComponentAdded);
        RemovedComponent.AddListener(OnComponentRemoved);
    }

    public void OnComponentAdded(T other)
    {
        EntityHealth entityHealth = other.gameObject.GetComponent<EntityHealth>();

        _healthComponentsArea.Add(entityHealth);

        entityHealth.DeathEvent.AddListener(RemoveDestroyedComponent);
    }

    public void OnComponentRemoved(T other)
    {
        EntityHealth entityHealth = other.gameObject.GetComponent<EntityHealth>();

        _healthComponentsArea.Remove(entityHealth);

        entityHealth.DeathEvent.RemoveListener(RemoveDestroyedComponent);
    }

    private void RemoveDestroyedComponent(GameObject other) => RemoveComponent(other.GetComponent<T>());
}
