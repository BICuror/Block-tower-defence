using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemToggleEffectContainer", menuName = "Item/ItemToggleEffectContainer")]

public sealed class ItemToggleEffectContainer : ScriptableObject
{
    [SerializeField] private List<ToggleGlobalEffectData> _itemToggleEffectDatas;
    
    public List<ToggleGlobalEffectData> EffectDatas => new(_itemToggleEffectDatas);
}