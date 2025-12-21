using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using System;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public sealed class ItemFactory : MonoBehaviour
{
    [Inject] private ItemsContainer _itemsContainer;
    [Inject] private DraggableCreator _draggableCreator;

    [SerializeField] private ToggleGlobalEffectData _startWaveEffectData; 
    [SerializeField] private ItemEffectSelector _effectSelector;
    [SerializeField] private List<Item> _itemsPrefabs;
    [SerializeField] private Item _waveItemPrefab;

    private List<Item> _createdItems = new();
    private ListDictionary<ItemColor, Item> _usedItemColors = new();
    
    public List<Item> CreatedItems => _createdItems;
    
#if UNITY_EDITOR
    [Button]
    public void CreateItems()
    {
        CreateItem(9, new Vector3(12f, 0f, 12));
        CreateItem(6, new Vector3(12f, 0f, 12));
    } 
    
#endif
    
    public async void CreateItem(int strength, Vector3 centerPosition)
    {
        Item itemPrefab = GetItemPrefab();
        
        DraggableObject itemDraggable = await _draggableCreator.CreateDraggableOnRandomPosition(itemPrefab, centerPosition);
        Item item = itemDraggable.GetComponent<Item>();
        
        item.SetStrength(strength);
        
        List<ToggleGlobalEffectData> toggleEfectDatas = _effectSelector.GetRandomToggleEffectDatas(strength);
        item.AddToggleEffectDatas(toggleEfectDatas);
        
        int charges = 0;
        toggleEfectDatas.ForEach(effectData => charges += effectData.Quality);
        
        item.SetChargesAmount(charges);
        
        _usedItemColors.Add(item.ItemColor, item);
        _createdItems.Add(item);

        item.ItemDestroyed += RemoveItem;
    }

    public async void CreateStartWaveItem(Vector3 centerPosition)
    {
        DraggableObject itemDraggable = await _draggableCreator.CreateDraggableOnRandomPosition(_waveItemPrefab, centerPosition);
        Item item = itemDraggable.GetComponent<Item>();

        List<ToggleGlobalEffectData> toggleEffectDatas = new List<ToggleGlobalEffectData>() {_startWaveEffectData};
        item.AddToggleEffectDatas(toggleEffectDatas);
    }

    public void DestroyAllUnusedItems()
    {
        List<Item> nonUsedItems = _createdItems.Except(_itemsContainer.ContainedItems).ToList();

        for (int i = 0; i < nonUsedItems.Count; i++)
        {
            RemoveItem(nonUsedItems[i]);
            nonUsedItems[i].DestroyItem();
        }
    }

    public void RemoveAndDestroyItem(Item item)
    {
        RemoveItem(item);
        item.DestroyItem();
    }
    
    private Item GetItemPrefab()
    {
        List<ItemColor> allItemColors = Enum.GetValues(typeof(ItemColor)).Cast<ItemColor>().ToList();

        List<ItemColor> usedItemColors = _usedItemColors.GetAllKeys();
        
        List<ItemColor> nonUsedItemColors = allItemColors.Except(usedItemColors).ToList();

        if (nonUsedItemColors.Count > 0)
        {
            ItemColor itemColor = nonUsedItemColors[Random.Range(0, nonUsedItemColors.Count)];
            
            return _itemsPrefabs.Find(item => item.ItemColor == itemColor);
        }
        
        return _itemsPrefabs[Random.Range(0, _itemsPrefabs.Count)];
    }
    
    private void RemoveItem(Item removedItem)
    {
        removedItem.ItemDestroyed -= RemoveItem;
        _usedItemColors.Remove(removedItem.ItemColor);
        _createdItems.Remove(removedItem);
    }
}

public enum ItemColor
{
    Red,
    Yellow,
    Blue,
    Purple
}