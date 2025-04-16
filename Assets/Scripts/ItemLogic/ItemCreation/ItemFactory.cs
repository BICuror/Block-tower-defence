using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class ItemFactory : MonoBehaviour
{
    [Inject] private DraggableCreator _draggableCreator;
    
    [SerializeField] private Item _itemPrefab; 
    [SerializeField] private ItemEffectSelector _effectSelector;

    private void Start()
    {
        CreateItem(1, 2, new Vector3(12, 4, 12));
    }

    public async void CreateItem(int quality, int strength, Vector3 centerPosition)
    {
        DraggableObject itemDraggable = await _draggableCreator.CreateDraggableOnRandomPosition(_itemPrefab, centerPosition);
        Item item = itemDraggable.GetComponent<Item>();
        
        int duration = 1;
        item.SetDuration(duration);
        
        List<ToggleGlobalEffectData> toggleEfectDatas = _effectSelector.GetRandomToggleEffectDatas(quality, strength);
        item.AddToggleEffectDatas(toggleEfectDatas);
        
        List<RewardGlobalEffectData> rewardDatas = _effectSelector.GetRandomRewardEffectDatas(quality, strength);
        item.AddRewardEffectDatas(rewardDatas);
    }
}
