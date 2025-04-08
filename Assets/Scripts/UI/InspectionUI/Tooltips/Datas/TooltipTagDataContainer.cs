using System.Collections.Generic;
using UnityEngine;

public abstract class TooltipTagDataContainer<T> : ScriptableObject where T : TooltipTagData
{
    [SerializeField] private List<T> _tooltipTags;
    
    public IReadOnlyList<T> TagDatas => _tooltipTags;
}