using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class ItemsContainer : MonoBehaviour
{
    [SerializeField] private ItemDetector _itemDetector;    
    [SerializeField] private Transform _parent;
    private List<Item> _items = new();

    public bool IsItemInspected => _items.Exists(item => item.GetComponent<Inspectable>().IsInspected);
    public List<Item> ContainedItems => _items;

    public Action ContainerUpdated;
    public Action<Item> ItemAdded;
    public Action<Item> ItemRemoved;
    public Action ItemInspectionStarted;
    public Action ItemInspectionEnded;

    private void Start()
    {
        _itemDetector.AddedItem += AddItem;
    }

    private void AddItem(Item item)
    {
        if (_items.Contains(item)) return;
        _items.Add(item);
    
        item.ItemPickedUp += RemoveItem;
        item.ItemDestroyed += RemoveItem;
        
        item.transform.SetParent(_parent);

        item.EnableToggleEffects();

        Inspectable inspectable = item.GetComponent<Inspectable>();
        
        inspectable.InspectionStarted += OnItemInspectionStarted;
        inspectable.InspectionEnded += OnItemInspectionEnded;

        ItemAdded?.Invoke(item);
        ContainerUpdated?.Invoke();
    }

    private void RemoveItem(Item item)
    {
        _items.Remove(item); 
        
        item.ItemPickedUp -= RemoveItem;
        item.ItemDestroyed -= RemoveItem;

        item.transform.SetParent(null);

        item.DisableToggleEffects();
        
        Inspectable inspectable = item.GetComponent<Inspectable>();
        
        inspectable.InspectionStarted -= OnItemInspectionStarted;
        inspectable.InspectionEnded -= OnItemInspectionEnded;
        
        ItemRemoved?.Invoke(item);
        ContainerUpdated?.Invoke();
    }
    
    private void OnItemInspectionStarted() => ItemInspectionStarted?.Invoke();
    private void OnItemInspectionEnded() => ItemInspectionEnded?.Invoke();
}