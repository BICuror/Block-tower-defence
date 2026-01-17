using UnityEngine;

public sealed class TokenContainer
{
    private int _tokenCount;
    
    public bool IsEmpty => _tokenCount == 0;
    public int TokenCount => _tokenCount;

    public void AddToken() => _tokenCount++;
    
    public void RemoveToken()
    {
        if (_tokenCount <= 0) Debug.LogError("Tried decreasing token count below zero. Undesired behaviour.");
        
        _tokenCount--;
    }
}