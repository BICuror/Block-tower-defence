using System.Collections.Generic;
using UnityEngine;
using TMPro;

public sealed class TooltipTextParser : MonoBehaviour
{
    private const char TOOLTIP_TEXT_START_CHAR = '#';
    private string TAG_START = "<#>";
    private string TAG_END = "</#>";

    [SerializeField] private TooltipParseDataContainer _tooltipParseDataContainer;
    [SerializeField] private List<StatTooltipData> _statTooltipDatas;
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    [TextArea] [SerializeField] private string _tooltipText;

    private void Awake()
    {
        _textMeshProUGUI.text = ParseTooltipText(_tooltipText);
    }
    
    public string ParseTooltipText(string tooltipText)
    {
        tooltipText = ParseStatTooltipText(tooltipText);   
        tooltipText = ParseByParseData(tooltipText);
        return tooltipText;
    }

    private string ParseStatTooltipText(string tooltipText)
    {
        _statTooltipDatas.ForEach(statData =>
        {
            string initialParseText = TOOLTIP_TEXT_START_CHAR + statData.AssociatedStatTypeName;
            
            Debug.Log(initialParseText);
            
            while (tooltipText.Contains(initialParseText))
            {
                string statFinalText = GetStringSpriteFromData(statData) + GetLink(statData.AssociatedStatTypeName, statData.StatName);
                
                tooltipText = tooltipText.Replace(initialParseText, statFinalText);
            }
        });

        return tooltipText;
    }

    private string GetStringSpriteFromData(StatTooltipData statTooltipData)
    {
        return $"<sprite name={statTooltipData.IconSprite.name}>";
    }

    private string GetLink(string linkName, string linkText)
    {
        return $"<link={'"'}{linkName}{'"'}>{linkText}</link>";
    }

    private string ParseByParseData(string tooltipText)
    {
        _tooltipParseDataContainer.ParseDartas.ForEach(parseData =>
        {
            string initialParseTagStart = TOOLTIP_TEXT_START_CHAR + parseData.InitialKey;
            
            while (tooltipText.Contains(initialParseTagStart))
            {
                string replaceStartValue = TAG_START.Replace(TOOLTIP_TEXT_START_CHAR.ToString(), parseData.ReplacedKey);
                
                tooltipText = ReplaceFirst(tooltipText, initialParseTagStart, replaceStartValue);
                
                string replaceEndValue = TAG_END.Replace(TOOLTIP_TEXT_START_CHAR.ToString(), parseData.ReplacedKey);
                string initialParseTagEnd = parseData.InitialKey + TOOLTIP_TEXT_START_CHAR;
                
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
}