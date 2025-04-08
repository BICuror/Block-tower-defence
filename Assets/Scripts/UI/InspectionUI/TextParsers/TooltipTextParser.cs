using System.Collections.Generic;
using UnityEngine;

public sealed class TooltipTextParser : MonoBehaviour
{
    private const char TOOLTIP_TAG_START_CHAR = '#';
    private string TAG_START = "<#>";
    private string TAG_END = "</#>";

    [SerializeField] private TooltipTextParseDataContainer _tooltipTextParseDataContainer;
    [SerializeField] private TooltipAllTagDataContainer _allTagDataContainer;
    
    public string ParseTooltipText(string tooltipText)
    {
        tooltipText = ParseTooltipTextByTagDatas(tooltipText);   
        tooltipText = ParseByParseData(tooltipText);
        
        return tooltipText;
    }

    #region TagParse

    private string ParseTooltipTextByTagDatas(string tooltipText)
    {
        IReadOnlyList<TooltipTagData> allTooltipTagDatas = _allTagDataContainer.GetAllTooltipTagDatas();
        
        foreach (TooltipTagData tagData in allTooltipTagDatas)
        {
            tooltipText = ParseByTooltipTagData(tooltipText, tagData);
        }

        return tooltipText;
    }

    private string ParseByTooltipTagData(string tooltipText, TooltipTagData tagData)
    {
        string initialParseText = TOOLTIP_TAG_START_CHAR + tagData.Tag;
        
        string finalText = GetStringSpriteFromData(tagData) + tagData.FinalText;

        while (tooltipText.Contains(initialParseText))
        {
            tooltipText = tooltipText.Replace(initialParseText, finalText);
        }
        
        return tooltipText;
    }
    
    private string GetStringSpriteFromData(TooltipTagData tagData)
    {
        return $"<sprite name={tagData.IconSprite.name}>";
    }

    #endregion

    #region StyleParsing
    
    private string ParseByParseData(string tooltipText) 
    { 
        _tooltipTextParseDataContainer.TextParseDatas.ForEach(parseData => 
        { 
            string initialParseTagStart = TOOLTIP_TAG_START_CHAR + parseData.InitialKey;
            
            while (tooltipText.Contains(initialParseTagStart)) 
            { 
                string replaceStartValue = TAG_START.Replace(TOOLTIP_TAG_START_CHAR.ToString(), parseData.ReplacedKey); 
                string replaceEndValue = TAG_END.Replace(TOOLTIP_TAG_START_CHAR.ToString(), parseData.ReplacedKey);
                
                string initialParseTagEnd = parseData.InitialKey + TOOLTIP_TAG_START_CHAR;
                
                tooltipText = ReplaceFirst(tooltipText, initialParseTagStart, replaceStartValue); 
                tooltipText = ReplaceFirst(tooltipText, initialParseTagEnd, replaceEndValue);
            }
        });
        
        return tooltipText;
    }
    
    private string ReplaceFirst(string text, string initialValue, string replacementValue) 
    { 
        int replacementIndex = text.IndexOf(initialValue);
        
        text = text.Remove(replacementIndex, initialValue.Length);
        
        text = text.Insert(replacementIndex, replacementValue); 
        
        return text;
    }

    #endregion
}