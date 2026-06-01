using CuroSettings;
using UnityEngine;

public abstract class ParserableTextContainer : MonoBehaviour
{
    [SerializeField] private bool _showFullTags;
    protected ReplaceableDataParser ReplaceableDataParser;
    protected TooltipDataParser TooltipDataParser;
    protected VisualTextParser VisualTextParser;

    private bool _parsersAreSet;
    
    public void CopyParsersFromContainer(ParserableTextContainer container)
    {
        SetParsers(container.ReplaceableDataParser, container.VisualTextParser, container.TooltipDataParser);
    }
    
    public void SetParsers(ReplaceableDataParser replaceableDataParser, VisualTextParser visualTextParser, TooltipDataParser tooltipDataParser)
    {
        if (_parsersAreSet) return;
        
        ReplaceableDataParser = replaceableDataParser;
        TooltipDataParser = tooltipDataParser;
        VisualTextParser = visualTextParser;

        ReplaceableDataParser.ReplaceableDataUpdated += UpdateAllParsableText;
        UpdateAllParsableText();

        _parsersAreSet = true;
    }
    
    protected virtual void UpdateAllParsableText() {}

    protected string ParseByReplacebleData(string text)
    {
        return ReplaceableDataParser.ParseReplaceableData(text);
    }   
    
    protected string ParseTextByDefault(string text)
    {
        text = ReplaceableDataParser.ParseReplaceableData(text);

        bool showFullTags = _showFullTags || SettingsContainer.GetSetting<BoolSetting>(SettingsEnum.FullTagsEnabled).Value;
        
        text = VisualTextParser.ParseTooltipText(text, showFullTags);
        
        return text;
    }

    protected void OnDestroy()
    {
        if (_parsersAreSet) ReplaceableDataParser.ReplaceableDataUpdated -= UpdateAllParsableText;
    }
}
