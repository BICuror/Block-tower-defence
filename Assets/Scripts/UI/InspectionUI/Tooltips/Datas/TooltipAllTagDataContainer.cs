using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatTooltipTagDataContainer", menuName = "Tooltips/AllTagsContainer")]

public sealed class TooltipAllTagDataContainer : ScriptableObject
{
    [SerializeField] private StatTooltipTagDataContainer _statTagDataContainer;
    [SerializeField] private KeywordTooltipTagDataContainer _keywordTagDataContainer;
    [SerializeField] private EffectTooltipTagDataContainer _effectTagDataContainer;

    public IReadOnlyList<StatTooltipTagData> StatTagDatas => _statTagDataContainer.TagDatas;
    public IReadOnlyList<KeywordTooltipTagData> KeywordTagDatas => _keywordTagDataContainer.TagDatas;
    public IReadOnlyList<EffectTooltipTagData> EffectTagDatas => _effectTagDataContainer.TagDatas;

    public IReadOnlyList<TooltipTagData> GetAllTooltipTagDatas()
    {
        List<TooltipTagData> allTooltipTagDatas = new();
        
        allTooltipTagDatas.AddRange(_statTagDataContainer.TagDatas);
        allTooltipTagDatas.AddRange(_keywordTagDataContainer.TagDatas);
        allTooltipTagDatas.AddRange(_effectTagDataContainer.TagDatas);

        return allTooltipTagDatas;
    }
}