using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemRewardContainer", menuName = "Item/ItemRewardContainer")]

public sealed class ItemRewardEffectCotainer : ScriptableObject
{
    [SerializeField] private List<RewardEffectData> _itemRewardEffectData;
    
    public List<RewardEffectData> EffectDatas => new List<RewardEffectData>(_itemRewardEffectData);
}