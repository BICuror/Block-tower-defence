using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemToggleEffectContainer", menuName = "Item/ItemToggleEffectContainer")]

public sealed class ItemToggleEffectContainer : ScriptableObject
{
    [SerializeField] private List<ToggleEffectData> _itemToggleEffectDatas;
    
    public List<ToggleEffectData> EffectDatas => new List<ToggleEffectData>(_itemToggleEffectDatas);
}