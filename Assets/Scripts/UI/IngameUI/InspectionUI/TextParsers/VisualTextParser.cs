using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class VisualTextParser : MonoBehaviour
{
    private readonly char[] ALLOWED_END_TAG_CHARACTERS = new[] { ',', ' ', '.', '=', ':' };
    private const string TOOLTIP_TAG_START_CHAR = "#";
    private const string TAG_START = "<#>";
    private const string TAG_END = "</#>";
    
    [SerializeField] private TooltipTextParseDataContainer _tooltipTextParseDataContainer;
    [SerializeField] private TooltipAllTagDataContainer _allTagDataContainer;
    
    private ReplaceableDataParser _replaceableDataParser;

    public string GetTagDescription(TooltipTagData tagData)
    {
        return GetTagHeaderWithoutIcon(tagData) + ": " + ParseTooltipText(tagData.Description);
    }
    
    public string GetTagHeaderWithoutIcon(TooltipTagData tagData, string tagReplacementText = null)
    {
        string tagHeaderText = tagData.TagText;

        if (tagReplacementText != null) tagHeaderText = tagReplacementText;
        
        return WrapInColor(ParseTooltipText(tagHeaderText), tagData.TextColor);
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
            if (ContainsFullTag(tooltipText, initialParseText))
            {
                string tagText = tagData.TagText;
                
                if (TryGetTagTextReplacement(initialParseText, ref tooltipText, out string tagTextReplacement)) tagText = tagTextReplacement;
                
                if (tagData.OnlyText) tooltipText = ReplaceFirst(tooltipText, initialParseText, GetTagHeaderWithoutIcon(tagData, tagText));
                else if (fullTag) tooltipText = ReplaceFirst(tooltipText, initialParseText, GetStringSpriteFromData(tagData) + GetTagHeaderWithoutIcon(tagData, tagText));
                else tooltipText = ReplaceFirst(tooltipText,initialParseText, GetStringSpriteFromData(tagData));
            }
            else break;
        }
        
        return tooltipText;
    }
    
    private string GetStringSpriteFromData(TooltipTagData tagData)
    {
        return $"<sprite name={tagData.IconSprite.name}>";
    }

    private bool ContainsFullTag(string parseText, string tagText)
    {
        if (TextEndsOnTag(parseText, tagText)) return true;

        for (int i = 0; i < ALLOWED_END_TAG_CHARACTERS.Length; i++)
        {
            if (parseText.Contains(tagText + ALLOWED_END_TAG_CHARACTERS[i])) return true;
        }

        return false;
    }

    private bool TextEndsOnTag(string parseText, string tagText)
    {
        return parseText.IndexOf(tagText) + tagText.Length == parseText.Length;
    }
    
    private bool TryGetTagTextReplacement(string initialParseText, ref string tooltipText, out string tagTextReplacement)
    {
        tagTextReplacement = null;
        
        string targetReplacementText = initialParseText + '=';
        
        int replacementTextIndex = tooltipText.IndexOf(targetReplacementText);
        
        if (replacementTextIndex < 0) return false;

        replacementTextIndex += targetReplacementText.Length;
        
        tagTextReplacement = tooltipText.Substring(replacementTextIndex).Split(' ')[0].Replace('_', ' ');
        tooltipText = tooltipText.Remove(replacementTextIndex - 1, tagTextReplacement.Length + 1);
        
        return true;
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
                string replaceStartValue = TAG_START.Replace(TOOLTIP_TAG_START_CHAR, parseData.ReplacedKey); 
                string replaceEndValue = TAG_END.Replace(TOOLTIP_TAG_START_CHAR, parseData.ReplacedKey);
                string initialParseTagEnd = parseData.InitialKey + TOOLTIP_TAG_START_CHAR;
                tooltipText = ReplaceFirst(tooltipText, initialParseTagEnd, replaceEndValue, false);
                tooltipText = ReplaceFirst(tooltipText, initialParseTagStart, replaceStartValue, false); 
            }
        });
        
        return tooltipText;
    }
    
    private string ReplaceFirst(string text, string initialValue, string replacementValue, bool addWhitespaceToEnd = true) 
    { 
        int replacementIndex = text.IndexOf(initialValue);

        if (replacementIndex == -1)
        {
            Debug.LogWarning(text);
            return text;
        }
        
        int length = initialValue.Length;

        if (length + replacementIndex + 1 < text.Length) length++;
        
        text = text.Remove(replacementIndex, length);

        if (addWhitespaceToEnd) replacementValue += ' ';
        
        text = text.Insert(replacementIndex, replacementValue); 
        
        return text;
    }

    #endregion

    #region ColorParsing
    
    private string WrapInColor(string initialString, Color color)
    {
        string colorCode = ColorUtility.ToHtmlStringRGB(color);
        
        return $"<color=#{colorCode}>{initialString}</color=#{colorCode}>";
    }

    #endregion
}