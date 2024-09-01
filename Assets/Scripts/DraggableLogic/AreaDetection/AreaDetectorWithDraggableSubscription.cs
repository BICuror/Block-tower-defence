using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AreaDetectorWithDraggableSubscription<T> : AreaDetector<T> where T: MonoBehaviour
{
    private List<DraggableObject> _placedDraggables = new List<DraggableObject>();
    private List<T> _placedComponents = new List<T>();

    public UnityEvent<T> PlacedComponentAdded;

    public UnityEvent<T> PlacedComponentRemoved;
    
    [SerializeField] private bool _TComponentHasHealth; 
    private List<EntityHealth> _componentsHealth = new List<EntityHealth>();

    public UnityEvent<EntityHealth> HealthComponentAdded;
    public UnityEvent<EntityHealth> HealthComponentRemoved;

    private void Awake()
    {
        AddedComponent.AddListener(OnComponentAdded);
        RemovedComponent.AddListener(OnComponentRemoved);
    }

    public void ClearPlacedList()
    {
        _componentsHealth = new List<EntityHealth>();
        _placedComponents = new List<T>();
        _placedDraggables = new List<DraggableObject>();
    }
    public bool HasPlacedComponents() => _placedComponents.Count > 0;

    public T GetFirstPlacedEntity() => _placedComponents[0];
    public IReadOnlyList<T> GetPlacedComponentsList() => _placedComponents;
    public IReadOnlyList<EntityHealth> GetHealthComponentsList() => _componentsHealth;
    public EntityHealth GetFirstHealthComponent() => _componentsHealth[0];

    public void OnComponentAdded(T other)
    {
        DraggableObject draggableObject = other.gameObject.GetComponent<DraggableObject>();
        
        if (draggableObject.IsPlaced())
        {
            AddPlacedDraggable(draggableObject);
        }
        else 
        {
            draggableObject.DraggablePlaced.AddListener(AddPlacedDraggable);
        }       
    }

    public void OnComponentRemoved(T other)
    {
        DraggableObject draggableObject = other.GetComponent<DraggableObject>();
        
        draggableObject.DraggablePlaced.RemoveListener(AddPlacedDraggable);
        draggableObject.DraggablePickedUp.RemoveListener(RemovePlacedDraggable);

        if (_TComponentHasHealth)
        {
            EntityHealth entityHealth = other.gameObject.GetComponent<EntityHealth>();

            _componentsHealth.Remove(entityHealth);
            entityHealth.DeathEvent.RemoveListener(RemoveDestroyedDraggable);
            HealthComponentRemoved.Invoke(entityHealth);
        }
        
        if (_placedComponents.Contains(other))
        {
            _placedComponents.Remove(other);
            _placedDraggables.Remove(draggableObject);

            PlacedComponentRemoved.Invoke(other);
        }
    }

    private void AddPlacedDraggable(DraggableObject draggable)
    {
        draggable.DraggablePlaced.RemoveListener(AddPlacedDraggable);
        
        _placedDraggables.Add(draggable);
        T component = draggable.GetComponent<T>();
        _placedComponents.Add(component);
        PlacedComponentAdded.Invoke(component);

        draggable.DraggablePickedUp.AddListener(RemovePlacedDraggable);

        if (_TComponentHasHealth)
        {
            EntityHealth entityHealth = draggable.gameObject.GetComponent<EntityHealth>();

            _componentsHealth.Add(entityHealth);
            draggable.gameObject.GetComponent<EntityHealth>().DeathEvent.AddListener(RemoveDestroyedDraggable);
            HealthComponentAdded.Invoke(entityHealth);
        }
    }

    private void RemovePlacedDraggable(DraggableObject draggable)
    {
        draggable.DraggablePickedUp.RemoveListener(RemovePlacedDraggable);

        draggable.DraggablePlaced.AddListener(AddPlacedDraggable);

        _placedDraggables.Remove(draggable);

        _placedDraggables.Remove(draggable);
        T component = draggable.GetComponent<T>();
        _placedComponents.Remove(component);
        PlacedComponentRemoved.Invoke(component);

        if (_TComponentHasHealth)
        {
            EntityHealth entityHealth = draggable.gameObject.GetComponent<EntityHealth>();

            _componentsHealth.Remove(entityHealth);
            entityHealth.DeathEvent.RemoveListener(RemoveDestroyedDraggable);
            HealthComponentRemoved.Invoke(entityHealth);
        }
    }

    private void RemoveDestroyedDraggable(GameObject other) => RemoveComponent(other.GetComponent<T>());
}
