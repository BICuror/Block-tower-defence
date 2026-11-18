using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine;
using System.Data;
using System.Linq;
using System;

namespace Combat.Stats
{
    public class StatContainer
    {
        private readonly Dictionary<Type, Stat> _stats = new();
    
        public void AddStats(StatInitializer[] statInitializers)
        {
            foreach (StatInitializer statInitializer in statInitializers)
            {
                Type statType = statInitializer.StatConfig.GetStatType();
    
                Stat statInstance = (Stat)Activator.CreateInstance(statType);
                statInstance.SetDefault(statInitializer.DefaultValue);
    
                AddStat(statInstance);
            }
        }
    
        public void RemoveStats(StatInitializer[] statInitializers)
        {
            foreach (StatInitializer statInitializer in statInitializers)
            {
                Type statType = statInitializer.StatConfig.GetStatType();
    
                Remove(statType);
            }
        }
    
        public List<Stat> GetAllStats()
        {
            return _stats.Values.ToList();
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

        public void AddStat(Type type, float defaultValue)
        {
            Stat stat = Activator.CreateInstance(type) as Stat;
            stat.SetDefault(defaultValue);
            
            AddStat(stat);
        }
        
        public void AddStat(Stat stat)
        {
            Type statType = stat.GetType();
    
            if (Has(statType) == false)
            {
                _stats.Add(stat.GetType(), stat);
            }
            else throw new DuplicateNameException($"Stat with type {statType.ToString()} already exists.");
        }

        public void AddStatIfDoesntExist<T>(float value) where T : Stat
        {
            if (!Has<T>())
            {
                T stat = Activator.CreateInstance<T>();
                stat.SetDefault(value);

                AddStat(stat);
            }
        }
    }
    
    [Serializable] public struct StatInitializer
    {
        [HideInInspector] public string StructName;
        [FormerlySerializedAs("_statData")] [SerializeField] private StatConfig _statConfig;
        [SerializeField] private float _defaultValue;
    
        public StatConfig StatConfig => _statConfig;
        public float DefaultValue => _defaultValue;
    }
}