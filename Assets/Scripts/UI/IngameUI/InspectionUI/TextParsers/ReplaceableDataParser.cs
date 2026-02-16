using System.Collections.Generic;
using System;

public sealed class ReplaceableDataParser
{
    private readonly Dictionary<string, string> _replaceableData = new();

    public event Action ReplaceableDataUpdated;
    
    public string ParseReplaceableData(string text)
    {
        foreach (string key in _replaceableData.Keys)
        {
            while (text.Contains(key))
            {
                text = text.Replace(key, _replaceableData[key]);
            }
        }
        
        return text;
    }

    public void AddOrUpdateParsableData(string key, string text)
    {
        _replaceableData.Add(key, text);
        
        ReplaceableDataUpdated?.Invoke();
    }

    public void RemoveParsableData(string key)
    {
        _replaceableData.Remove(key);
        
        ReplaceableDataUpdated?.Invoke();
    }
}