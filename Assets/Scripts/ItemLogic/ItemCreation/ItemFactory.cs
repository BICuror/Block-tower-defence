using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using WorldGeneration;
using System.Linq;
using UnityEngine;
using Zenject;
using System;
using Random = UnityEngine.Random;

public sealed class ItemFactory : MonoBehaviour
{
    [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
    [Inject] private DraggableCreator _draggableCreator;
    [Inject] private ItemsContainer _itemsContainer;

    [SerializeField] private float _creationRadius = 3;
    [SerializeField] private GlobalEffectData _startWaveEffectData; 
    [SerializeField] private ItemEffectSelector _effectSelector;
    [SerializeField] private List<Item> _itemsPrefabs;
    [SerializeField] private Item _waveItemPrefab;

    private List<Item> _createdItems = new();
    private ListDictionary<ItemColor, Item> _usedItemColors = new();
    
    public List<Item> CreatedItems => _createdItems;
    
#if UNITY_EDITOR
    [Button]
    public void DEBUGCreateItems()
    {
        List<List<GlobalEffectData>> globalEffects = _effectSelector.GetItemEffects(11, 1, 4);
            
        CreateItemFromEffects(globalEffects[0], new Vector3(12f, 0f, 12), Vector3.zero).Forget();
        CreateItemFromEffects(globalEffects[1], new Vector3(12f, 0f, 12), Vector3.zero).Forget();
        CreateItemFromEffects(globalEffects[2], new Vector3(12f, 0f, 12), Vector3.zero).Forget();
        CreateItemFromEffects(globalEffects[3], new Vector3(12f, 0f, 12), Vector3.zero).Forget();
    } 
#endif
    
    public async UniTask CreateItems(Vector3 position, int totalStrength, int minimalItemStrength, int itemAmount, bool createStartWaveItem = false)
    {
        List<List<GlobalEffectData>> globalEffects = _effectSelector.GetItemEffects(totalStrength, minimalItemStrength, itemAmount);
        
        int totalItemAmount = itemAmount;
        
        if (createStartWaveItem) totalItemAmount += 1;
        
        float angleStep = 360f / totalItemAmount;
        
        float offset = Random.Range(0, 360f);
        
        for (int i = 0; i < totalItemAmount; i++)
        {
            bool isStartWaveItem = i == itemAmount;
            
            Vector3 finalPosition = GetOffset(i) + position;

            if (!isStartWaveItem) await CreateItemFromEffects(globalEffects[i], position, finalPosition);
            else await CreateStartWaveItem(position, finalPosition);
        }
        
        return;
        
        Vector3 GetOffset(int index)
        {
            float xOffset = Mathf.Cos(Mathf.Deg2Rad * (angleStep * index + offset)) * _creationRadius;
            float zOffset = Mathf.Sin(Mathf.Deg2Rad * (angleStep * index + offset)) * _creationRadius;

            return new Vector3(xOffset, 0f, zOffset);
        }
    }
    
    public async UniTask CreateItem(int strength, Vector3 position)
    {
        _effectSelector.TryGetItemEffectDatas(strength, new(), false, out List<GlobalEffectData> effectDatas);
        
        await CreateItemFromEffects(effectDatas, position, Vector3.zero);
    }

    private async UniTask CreateItemFromEffects(List<GlobalEffectData> effectDatas, Vector3 position, Vector3 desiredPosition)
    {
        Item itemPrefab = GetItemPrefab();
        DraggableObject itemDraggable;
        if (desiredPosition == Vector3.zero) itemDraggable = await _draggableCreator.CreateDraggableOnRandomPosition(itemPrefab, position);
        else itemDraggable = await _draggableCreator.CreateDraggableOnNearbyPosition(itemPrefab, position, desiredPosition);
        
        Item item = itemDraggable.GetComponent<Item>();
        
        item.AddToggleEffectDatas(effectDatas);
        
        int charges = 0;
        effectDatas.ForEach(effectData => charges += effectData.Quality);
        item.SetChargesAmount(charges);
        
        _usedItemColors.Add(item.ItemColor, item);
        _createdItems.Add(item);

        item.ItemDestroyed += RemoveItem;
    }

    public async UniTask<Item> CreateStartWaveItem(Vector3 centerPosition, Vector3 finalPosition)
    {
        DraggableObject itemDraggable = await _draggableCreator.CreateDraggableOnNearbyPosition(_waveItemPrefab, centerPosition, finalPosition);
        Item item = itemDraggable.GetComponent<Item>();

        List<GlobalEffectData> effectDatas = new List<GlobalEffectData>() {_startWaveEffectData};
        item.AddToggleEffectDatas(effectDatas);

        return item;
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

        List<ItemColor> nonUsedItemColors = allItemColors.Except(_usedItemColors.GetAllKeys()).ToList();

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
    Purple
}