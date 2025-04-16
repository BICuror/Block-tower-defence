using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemRewardContainer", menuName = "Item/ItemRewardContainer")]

public sealed class ItemRewardEffectCotainer : ScriptableObject
{
    [SerializeField] private List<RewardGlobalEffectData> _itemRewardEffectData;
    
    public List<RewardGlobalEffectData> EffectDatas => new List<RewardGlobalEffectData>(_itemRewardEffectData);
}