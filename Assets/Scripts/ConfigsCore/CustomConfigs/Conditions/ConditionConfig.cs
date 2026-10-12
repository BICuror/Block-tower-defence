using System.Collections.Generic;
using UnityEngine;
using System;

namespace Configs
{
    [CreateAssetMenu(menuName = "Configs/ConditionConfig")]
    
    public sealed class ConditionConfig : TypeConfig {}
    
    [Serializable] public sealed class ConditionConfigEntry : ArgumentDataEntryContainer
    {
        [SerializeField] private ConditionConfig _config;
        
        public ConditionConfig Config => _config;
        public Type GetContainedType() => _config.GetContainedType();
        
        protected override bool DisplayArguments => _config;
        protected override Type GetCurrentTargetType() => _config.GetContainedType();
        
        public void ResetConfig() => _config = null;
        
        public override List<INonScriptableObjectUpdatable> GetAllUpdatables()
        {
            List<INonScriptableObjectUpdatable> updatables = base.GetAllUpdatables();
            
            updatables.Add(this);

            return updatables;
        }

        public override void Update()
        {
            base.Update();

            if (_config) Name = _config.name;
            else Name = "No config";
        }
    }
        
    [Serializable] public sealed class ConditionalExecutionEntry
    {
        [SerializeField] private ConditionLogic _requiredConditionLogic;
        [SerializeField] private List<ConditionConfigEntry> _conditions;
        
        public List<ConditionConfigEntry> ConditionEntries => _conditions;
        public ConditionLogic RequiredConditionLogic => _requiredConditionLogic;

        public IEnumerable<INonScriptableObjectUpdatable> GetAllUpdatables()
        {
            List<INonScriptableObjectUpdatable> updatables = new();

            foreach (ConditionConfigEntry conditionConfigEntry in _conditions)
            {
                updatables.Add(conditionConfigEntry);
            }
            
            return updatables;
        }
    }

    public enum ConditionLogic
    {
        All,
        Any
    }
}