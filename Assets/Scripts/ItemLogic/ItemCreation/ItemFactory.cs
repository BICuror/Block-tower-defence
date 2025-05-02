using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class ItemFactory : MonoBehaviour
{
    [Inject] private DraggableCreator _draggableCreator;
    
    [SerializeField] private ToggleGlobalEffectData _startWaveEffectData;
    [SerializeField] private Item _itemPrefab; 
    [SerializeField] private ItemEffectSelector _effectSelector;

    private void Start()
    {
        CreateStartWaveItem(new Vector3(12, 4, 12));
    }

    public async void CreateItem(int quality, int strength, Vector3 centerPosition)
    {
        DraggableObject itemDraggable = await _draggableCreator.CreateDraggableOnRandomPosition(_itemPrefab, centerPosition);
        Item item = itemDraggable.GetComponent<Item>();
        
        int duration = 1;
        item.SetDuration(duration);
        item.SetItemData(quality, strength);
        
        List<ToggleGlobalEffectData> toggleEfectDatas = _effectSelector.GetRandomToggleEffectDatas(quality, strength);
        item.AddToggleEffectDatas(toggleEfectDatas);
        
        List<RewardGlobalEffectData> rewardDatas = _effectSelector.GetRandomRewardEffectDatas(quality, strength);
        item.AddRewardEffectDatas(rewardDatas);
    }

    public async void CreateStartWaveItem(Vector3 centerPosition)
    {
        DraggableObject itemDraggable = await _draggableCreator.CreateDraggableOnRandomPosition(_itemPrefab, centerPosition);
        Item item = itemDraggable.GetComponent<Item>();
        
        item.SetItemData(0, 1);
        
        List<ToggleGlobalEffectData> toggleEfectDatas = _effectSelector.GetRandomToggleEffectDatas(0, 1);
        toggleEfectDatas.Add(_startWaveEffectData);
        item.AddToggleEffectDatas(toggleEfectDatas);
    }
}
