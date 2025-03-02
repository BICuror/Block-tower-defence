using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Tooltips/ParseDataContainer")]
public sealed class TooltipParseDataContainer : ScriptableObject
{
    [SerializeField] private List<ParsedData> _parseDartas;

    public List<ParsedData> ParseDartas => _parseDartas;
}

[System.Serializable] public sealed class ParsedData
{
    public string InitialKey;
    public string ReplacedKey;
}