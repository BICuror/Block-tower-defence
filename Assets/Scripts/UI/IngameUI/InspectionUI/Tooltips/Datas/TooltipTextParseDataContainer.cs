using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tooltips/TextParseDataContainer")]

public sealed class TooltipTextParseDataContainer : ScriptableObject
{
    [SerializeField] private List<ParsedData> _textParseDatas;
    [SerializeField] private List<ParsedData> _replaceableParseDatas;

    public List<ParsedData> TextParseDatas => _textParseDatas;
    public List<ParsedData> ReplaceableParseDatas => _replaceableParseDatas;
}

[System.Serializable] public sealed class ParsedData
{
    public string InitialKey;
    public string ReplacedKey;
}