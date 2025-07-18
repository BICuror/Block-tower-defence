using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;
using UnityEngine.Serialization;

[Serializable] public sealed class InstanceItemTypeContainer
{
    [Dropdown("AllEffectTypeNames")] [AllowNesting] [SerializeField] private string _itemTypeName;
    
    [HideInInspector] public List<string> AllEffectTypeNames;
    
    public Type InstanceType => Type.GetType(_itemTypeName);
}