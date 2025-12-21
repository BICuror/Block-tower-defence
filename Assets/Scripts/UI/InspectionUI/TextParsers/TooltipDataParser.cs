using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class TooltipDataParser : MonoBehaviour
{
    private const char TOOLTIP_TAG_START_CHAR = '#';
 
    [SerializeField] private TooltipAllTagDataContainer _allTagDataContainer;
    
    public TooltipParseTagDataContainer GetTooltipTagDataFromText(string tooltipText)
    {
        TooltipParseTagDataContainer parseDataContainer = new();
     
        List<string> tooltipTextParts = tooltipText.Split(TOOLTIP_TAG_START_CHAR).ToList();
        
        tooltipTextParts.ForEach(tooltipTextPart =>
        {
            ParseTags(TOOLTIP_TAG_START_CHAR + tooltipTextPart, parseDataContainer, _allTagDataContainer.EffectTagDatas);   
            ParseTags(TOOLTIP_TAG_START_CHAR + tooltipTextPart, parseDataContainer, _allTagDataContainer.StatTagDatas);   
            ParseTags(TOOLTIP_TAG_START_CHAR + tooltipTextPart, parseDataContainer, _allTagDataContainer.KeywordTagDatas);   
        });
        
        return parseDataContainer;
    }
    
    private void ParseTags(string tooltipText, TooltipParseTagDataContainer parseDataContainer, IReadOnlyList<TooltipTagData> tagDataList)
    {
        foreach (TooltipTagData tagData in tagDataList)
        {
            if (tooltipText.Contains(TOOLTIP_TAG_START_CHAR + tagData.Tag))
            {
                parseDataContainer.TagDatas.Add(tagData);
            }
        }
    }
}