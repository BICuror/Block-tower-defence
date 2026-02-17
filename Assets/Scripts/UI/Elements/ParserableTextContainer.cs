using UnityEngine;

public abstract class ParserableTextContainer : MonoBehaviour
{
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

    protected string ParseTextByDefault(string text)
    {
        text = ReplaceableDataParser.ParseReplaceableData(text);
        text = VisualTextParser.ParseTooltipText(text);
        
        return text;
    }

    protected void OnDestroy()
    {
        if (_parsersAreSet) ReplaceableDataParser.ReplaceableDataUpdated -= UpdateAllParsableText;
    }
}
