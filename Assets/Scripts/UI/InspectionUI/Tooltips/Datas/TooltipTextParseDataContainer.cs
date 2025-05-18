using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Tooltips/TextParseDataContainer")]

public sealed class TooltipTextParseDataContainer : ScriptableObject
{
    [SerializeField] private List<ParsedData> _textParseDatas;

    public List<ParsedData> TextParseDatas => _textParseDatas;
}

[System.Serializable] public sealed class ParsedData
{
    public string InitialKey;
    public string ReplacedKey;
}