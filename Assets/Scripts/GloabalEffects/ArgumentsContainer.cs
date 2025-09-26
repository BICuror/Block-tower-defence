using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;

[Serializable] public sealed class ArgumentsContainer
{
    [Header("Arguments Container")] 
    [SerializeField] private List<ArgumentContainerItem> _argumentItems;
    
    public bool HasArgument(string argumentName)
    {
        return _argumentItems.Exists(item => item.ArgumentName == argumentName);
    }
    
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
    [AllowNesting] [ShowIf("_argumentType", ArgumentType.Bool)] [SerializeField] private bool _boolArgument;
    [AllowNesting] [ShowIf("_argumentType", ArgumentType.Float)] [SerializeField] private float _floatArgument;
    [AllowNesting] [ShowIf("_argumentType", ArgumentType.String)] [SerializeField] private string _stringArgument;
    [AllowNesting] [ShowIf("_argumentType", ArgumentType.GameObject)] [SerializeField] private GameObject _gameObjectArgument;
    [AllowNesting] [ShowIf("_argumentType", ArgumentType.EntityModificatorData)] [SerializeField] private EntityModificatorData _entityModificatorData;
    [AllowNesting] [ShowIf("_argumentType", ArgumentType.EntityEffectParticleHandler)] [SerializeField] private EntityEffectParticleHandler _entityEffectParticleHandler;
    [AllowNesting] [ShowIf("_argumentType", ArgumentType.EntityObjectModifier)] [SerializeField] private EntityObjectModifier _entityObjectModifierPrefab;
    
    public string ArgumentName => _argumentName;

    public object GetValue()
    {
        switch (_argumentType)
        {
            case ArgumentType.Int: return _intArgument;
            case ArgumentType.Bool: return _boolArgument;
            case ArgumentType.Float: return _floatArgument;
            case ArgumentType.String: return _stringArgument;
            case ArgumentType.GameObject: return _gameObjectArgument;
            case ArgumentType.EntityModificatorData: return _entityModificatorData;
            case ArgumentType.EntityEffectParticleHandler: return _entityEffectParticleHandler;
            case ArgumentType.EntityObjectModifier: return _entityObjectModifierPrefab;
            default: throw new NotImplementedException($"Unknown argument type: {_argumentType}");
        }
    }
}

public enum ArgumentType
{
    Int,
    Float,
    GameObject,
    EntityModificatorData,
    String,
    Bool,
    EntityEffectParticleHandler,
    EntityObjectModifier,
}