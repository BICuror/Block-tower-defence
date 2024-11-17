using UnityEngine;
using System.Collections.Generic;
using System;
using System.Data;

public sealed class StatContainer : MonoBehaviour
{
    [SerializeField] private StatInitializer[] _statInitializers;
    private readonly Dictionary<Type, Stat> _stats = new();

    public void Initialize()
    {
        foreach (StatInitializer statInitializer in _statInitializers)
        {
            Type statType = statInitializer.StatData.GetStatType();

            Stat statInstance = (Stat)Activator.CreateInstance(statType);
            statInstance.SetDefault(statInitializer.DefaultValue);

            AddStat(statInstance);
        }
    }

    #region Generic
    public T Get<T>() where T : Stat
    {
        return (T)Get(typeof(T));
    }
    public bool Has<T>() where T : Stat
    {
        return Has(typeof(T));
    }
    public void Remove<T>() where T : Stat
    {
        Remove(typeof(T));
    }
    #endregion
    
    #region Typed
    public Stat Get(Type type)
    {
        if (_stats[type] != null) return _stats[type];
        
        throw new KeyNotFoundException($"Stat with type {type.ToString()} was found.");
    }
    public bool Has(Type type) => _stats.ContainsKey(type);
    public void Remove(Type type)
    {
        if (Has(type)) _stats.Remove(type);
        else throw new KeyNotFoundException($"Stat with type {type.ToString()} was not found.");
    }
    #endregion
    
    public void AddStat(Stat stat)
    {
        Type statType = stat.GetType();

        if (Has(statType) == false)
        {
            _stats.Add(stat.GetType(), stat);
        }
        else throw new DuplicateNameException($"Stat with type {statType.ToString()} already exists.");
    }
    
    private void OnValidate()
    {
        try
        {
            for (int i = 0; i < _statInitializers.Length; i++)
            {
                _statInitializers[i].StructName = _statInitializers[i].StatData.GetStatType().ToString();

                Type statType = _statInitializers[i].StatData.GetStatType();
                
                if (Has(statType))
                {
                    Get(statType).SetDefault(_statInitializers[i].DefaultValue);
                }
            }
        }
        catch (Exception ex) { Debug.LogWarning(ex.Message); }
    }

    [Serializable] private struct StatInitializer
    {
        [HideInInspector] public string StructName;
        [SerializeField] private StatData _statData;
        [SerializeField] private float _defaultValue;

        public StatData StatData => _statData;
        public float DefaultValue => _defaultValue;
    }
}