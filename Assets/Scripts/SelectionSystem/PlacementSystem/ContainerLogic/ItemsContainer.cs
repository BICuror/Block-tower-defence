using System.Collections.Generic;
using UnityEngine;
using System;

using Random = UnityEngine.Random;

public sealed class ItemsContainer : MonoBehaviour
{
    [SerializeField] private ItemDetector _itemDetector;    
    [SerializeField] private Transform _parent;
    private List<Item> _items = new();

    public List<Item> ContainedItems => new List<Item>(_items);

    public Action ContainerUpdated;
    public Action<Item> ItemAdded;
    public Action<Item> ItemRemoved;

    private void Start()
    {
        _itemDetector.AddedItem += AddItem;
    }

    private void AddItem(Item item)
    {
        if (_items.Contains(item)) return;
        _items.Add(item);
    
        item.ItemPickedUp += RemoveItem;
        item.DurationEnded += RemoveItem;
        
        item.transform.SetParent(_parent);

        item.EnableProperties();

        ItemAdded?.Invoke(item);
        ContainerUpdated?.Invoke();
    }

    private void RemoveItem(Item item)
    {
        _items.Remove(item); 
        
        item.ItemPickedUp -= RemoveItem;
        item.DurationEnded -= RemoveItem;

        item.transform.SetParent(null);

        item.DisableProperties();
        
        ItemRemoved?.Invoke(item);
        ContainerUpdated?.Invoke();
    }
}