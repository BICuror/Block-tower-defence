using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;

[Serializable] public sealed class ArgumentsContainer
{
    [Header("Arguments Container")] 
    [SerializeField] private List<ArgumentContainerItem> _argumentItems;

    public T GetArgument<T>(string argumentName)
    {
        ArgumentContainerItem item = _argumentItems.Find(item => item.ArgumentName == argumentName);

        return (T)item.GetValue();
    }
}

[Serializable] public sealed class ArgumentContainerItem
{
    [SerializeField] private ArgumentType _argumentType;
    [SerializeField] private string _argumentName;

    [AllowNesting] [ShowIf("_argumentType", ArgumentType.Int)] [SerializeField] private int _intArgument;
    [AllowNesting] [ShowIf("_argumentType", ArgumentType.Float)] [SerializeField] private float _floatArgument;
    [AllowNesting] [ShowIf("_argumentType", ArgumentType.GameObject)] [SerializeField] private GameObject _gameObjectArgument;
    
    public string ArgumentName => _argumentName;

    public object GetValue()
    {
        switch (_argumentType)
        {
            case ArgumentType.Int: return _intArgument;
            case ArgumentType.Float: return _floatArgument;
            case ArgumentType.GameObject: return _gameObjectArgument;
            default: throw new NotImplementedException($"Unknown argument type: {_argumentType}");
        }
    }
}

public enum ArgumentType
{
    Int,
    Float,
    GameObject
}