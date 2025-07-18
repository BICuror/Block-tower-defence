using UnityEngine;

public sealed class TooltipDataParser : MonoBehaviour
{
    private const char TOOLTIP_TAG_START_CHAR = '#';
 
    [SerializeField] private TooltipAllTagDataContainer _allTagDataContainer;
    
    public TooltipParseTagDataContainer GetTooltipTagDataFromText(string tooltipText)
    {
        TooltipParseTagDataContainer parseDataContainer = new();
        
        ParseByStatTags(tooltipText, parseDataContainer);   
        ParseByEffectsTags(tooltipText, parseDataContainer);   
        ParseByKeywordTags(tooltipText, parseDataContainer);   
        
        return parseDataContainer;
    }

    private void ParseByStatTags(string tooltipText, TooltipParseTagDataContainer parseDataContainer)
    {
        foreach (StatTooltipTagData tagData in _allTagDataContainer.StatTagDatas)
        {
            if (tooltipText.Contains(TOOLTIP_TAG_START_CHAR + tagData.Tag))
            {
                parseDataContainer.StatTagDatas.Add(tagData);
            }
        }
    }
    
    private void ParseByEffectsTags(string tooltipText, TooltipParseTagDataContainer parseDataContainer)
    {
        foreach (EffectTooltipTagData tagData in _allTagDataContainer.EffectTagDatas)
        {
            if (tooltipText.Contains(TOOLTIP_TAG_START_CHAR + tagData.Tag))
            {
                parseDataContainer.EffectTagDatas.Add(tagData);
            }
        }
    }
    
    private void ParseByKeywordTags(string tooltipText, TooltipParseTagDataContainer parseDataContainer)
    {
        foreach (KeywordTooltipTagData tagData in _allTagDataContainer.KeywordTagDatas)
        {
            if (tooltipText.Contains(TOOLTIP_TAG_START_CHAR + tagData.Tag))
            {
                parseDataContainer.KeywordTagDatas.Add(tagData);
            }
        }
    }
}