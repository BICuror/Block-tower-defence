using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using Zenject;
using System;

using Random = UnityEngine.Random;

public sealed class ItemFactory : MonoBehaviour
{
    [Inject] private DraggableCreator _draggableCreator;
    [Inject] private ItemsContainer _itemsContainer;

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
        List<List<GlobalEffectData>> globalEffects = _effectSelector.GetItemEffects(11, 3, 2);
            
        CreateItemFromEffects(globalEffects[0], new Vector3(12f, 0f, 12)).Forget();
        CreateItemFromEffects(globalEffects[1], new Vector3(12f, 0f, 12)).Forget();
    } 
#endif
    
    public async UniTask CreateItems(Vector3 position, int totalStrength, int minimalItemStrength, int itemAmount)
    {
        List<List<GlobalEffectData>> globalEffects = _effectSelector.GetItemEffects(totalStrength, minimalItemStrength, itemAmount);
            
        for (int i = 0; i < globalEffects.Count; i++)
        {
            await CreateItemFromEffects(globalEffects[i], position);
        }
    }
    
    public async UniTask CreateItem(int strength, Vector3 position)
    {
        _effectSelector.TryGetItemEffectDatas(strength, new(), false, out List<GlobalEffectData> effectDatas);
        
        await CreateItemFromEffects(effectDatas, position);
    }

    private async UniTask CreateItemFromEffects(List<GlobalEffectData> effectDatas, Vector3 position)
    {
        Item itemPrefab = GetItemPrefab();
        DraggableObject itemDraggable = await _draggableCreator.CreateDraggableOnRandomPosition(itemPrefab, position);
        Item item = itemDraggable.GetComponent<Item>();
        
        item.AddToggleEffectDatas(effectDatas);
        
        int charges = 0;
        effectDatas.ForEach(effectData => charges += effectData.Quality);
        item.SetChargesAmount(charges);
        
        _usedItemColors.Add(item.ItemColor, item);
        _createdItems.Add(item);

        item.ItemDestroyed += RemoveItem;
    }

    public async UniTask CreateStartWaveItem(Vector3 centerPosition)
    {
        DraggableObject itemDraggable = await _draggableCreator.CreateDraggableOnRandomPosition(_waveItemPrefab, centerPosition);
        Item item = itemDraggable.GetComponent<Item>();

        List<GlobalEffectData> effectDatas = new List<GlobalEffectData>() {_startWaveEffectData};
        item.AddToggleEffectDatas(effectDatas);
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