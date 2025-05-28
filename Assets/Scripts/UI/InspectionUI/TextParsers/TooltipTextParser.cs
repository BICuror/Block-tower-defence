using System.Collections.Generic;
using UnityEngine;

public sealed class TooltipTextParser : MonoBehaviour
{
    private const char TOOLTIP_TAG_START_CHAR = '#';
    private string TAG_START = "<#>";
    private string TAG_END = "</#>";

    [SerializeField] private TooltipTextParseDataContainer _tooltipTextParseDataContainer;
    [SerializeField] private TooltipAllTagDataContainer _allTagDataContainer;

    public string GetTagDescription(TooltipTagData tagData)
    {
        return WrapInColor(tagData.TagText + ": ", tagData.TextColor) + ParseTooltipText(tagData.Description);
    }
    
    public string GetTagHeaderWithoutIcon(TooltipTagData tagData)
    {
        return WrapInColor(tagData.TagText, tagData.TextColor);
    }
    
    public string GetDefaultTagHeader(TooltipTagData tagData)
    {
        return GetStringSpriteFromData(tagData) + GetTagHeaderWithoutIcon(tagData);
    }
    
    public string ParseTooltipText(string tooltipText, bool fullTag = true)
    {
        tooltipText = ParseTooltipTextByTagDatas(tooltipText, fullTag);   
        tooltipText = ParseByParseData(tooltipText);
        
        return tooltipText;
    }

    #region TagParse

    private string ParseTooltipTextByTagDatas(string tooltipText, bool fullTag = true)
    {
        IReadOnlyList<TooltipTagData> allTooltipTagDatas = _allTagDataContainer.GetAllTooltipTagDatas();
        
        foreach (TooltipTagData tagData in allTooltipTagDatas)
        {
            tooltipText = ParseByTooltipTagData(tooltipText, tagData, fullTag);
        }

        return tooltipText;
    }

    private string ParseByTooltipTagData(string tooltipText, TooltipTagData tagData, bool fullTag = true)
    {
        string initialParseText = TOOLTIP_TAG_START_CHAR + tagData.Tag;

        while (tooltipText.Contains(initialParseText))
        {
            if (fullTag)
            {
                tooltipText = tooltipText.Replace(initialParseText, GetDefaultTagHeader(tagData));
            }
            else
            {
                tooltipText = tooltipText.Replace(initialParseText, GetStringSpriteFromData(tagData));
            }
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

        int length = initialValue.Length;

        if (length + replacementIndex + 1 < text.Length) length++;
        
        text = text.Remove(replacementIndex, length);
        
        text = text.Insert(replacementIndex, replacementValue); 
        
        return text;
    }

    #endregion

    #region ColorParsing
    
    private string WrapInColor(string initialString, Color color)
    {
        string colorCode = ColorUtility.ToHtmlStringRGB(color);
        
        return $"<color=#{colorCode}>{initialString}</color>";
    }

    #endregion
}