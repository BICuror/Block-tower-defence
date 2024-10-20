using UnityEngine;
using System.Collections.Generic;
using System;

public sealed class StatContainer : MonoBehaviour
{
    [SerializeField] private StatInitializer[] _statInitializers;
    private Dictionary<Type, Stat> _stats = new();

    private void Awake()
    {
        foreach (StatInitializer statInitializer in _statInitializers)
        {
            Type statType = statInitializer.StatData.GetStatType();

            Stat statInstance = (Stat)Activator.CreateInstance(statType);
            statInstance.SetDefault(statInitializer.DefaultValue);

            AddStat(statInstance);
        }
    }

    public T GetStat<T>() where T : Stat
    {
        Type type = typeof(T);

        T result = _stats[type] as T;

        if (result != null) Debug.Log($"{gameObject.name} doesn't have stat with type {type.ToString()}");

        return result;
    }

    public bool HasStat(Type type) => _stats.ContainsKey(type);

    public void AddStat(Stat stat)
    {
        Type statType = stat.GetType();

        if (HasStat(statType) == false)
        {
            _stats.Add(stat.GetType(), stat);
            Debug.Log($"Added {stat.GetType().ToString()} stat to {gameObject.name}");
        }
        else Debug.LogError($"Tried adding additional instance of {statType.ToString()} to {gameObject.name}");
    }

    public void RemoveStat(Type type)
    {
        if (HasStat(type)) _stats.Remove(type);
        else Debug.LogError($"Tried removing to {type.ToString()} to {gameObject.name} while not having it in the first place");
    }

    private void OnValidate()
    {
        try
        {
            for (int i = 0; i < _statInitializers.Length; i++)
            {    
                _statInitializers[i].StructName = _statInitializers[i].StatData.GetStatType().ToString();
            }
        }
        catch (Exception ex) {}
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