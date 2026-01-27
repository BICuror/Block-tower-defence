using UnityEngine;

public sealed class TokenContainer
{
    private bool _canBeNullReduced; 
    private int _tokenCount;
    
    public bool IsEmpty => _tokenCount == 0;
    public int TokenCount => _tokenCount;

    public TokenContainer(bool canBeNullReduced)
    {
        _canBeNullReduced = canBeNullReduced;
    }
    
    public void AddToken() => _tokenCount++;
    
    public void RemoveToken()
    {
        if (_tokenCount <= 0)
        {
            if (!_canBeNullReduced) Debug.LogError("Tried decreasing token count below zero. Undesired behaviour.");
        }
        else _tokenCount--;
    }
}