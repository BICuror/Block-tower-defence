using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemEffectContainer", menuName = "Item/ItemEffectContainer")]

public sealed class ItemToggleEffectContainer : ScriptableObject
{
    [SerializeField] private List<GlobalEffectData> _itemEffectDatas;
    
    public List<GlobalEffectData> EffectDatas => new(_itemEffectDatas);
}