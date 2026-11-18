using System.Collections.Generic;
using Combat.Entity.Modificators;
using UnityEngine;
using System;
using Ligofff.CustomSOIcons;

namespace Combat.Entity.Sets
{
    [CreateAssetMenu(fileName = "EntitySetConfigContainer", menuName = "EntitySets/EntitySetConfig")]
    
    public sealed class EntitySetConfig : ScriptableObject
    {
        [SerializeField] private List<EntitySetThresholdConfig> _thresholdDatas;

        [Header("UI")] 
        [SerializeField] private Sprite _setIcon;
        [SerializeField] private string _setLocKey;

        public IReadOnlyList<EntitySetThresholdConfig> ThresholdConfigs => _thresholdDatas;
        
        [CustomAssetIcon] public Sprite SetIcon => _setIcon;
        public string SetNameLocKey => _setLocKey;
    }

    [Serializable] public sealed class EntitySetThresholdConfig
    {
        [SerializeField] private int _levelThreshold;
        [SerializeField] private string _thresholdDescriptionEffectLocKey;
        [SerializeField] private EntityModificatorConfig _entityModificator;
        
        public int LevelThreshold => _levelThreshold;
        public string ThresholdDescriptionEffectLocKey => _thresholdDescriptionEffectLocKey;
        public EntityModificatorConfig EntityModificator => _entityModificator;
    }
}