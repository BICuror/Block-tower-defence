using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;

namespace Configs
{
    [Serializable] public abstract class ConfigEntry<TConfig> : ArgumentDataEntryContainer where TConfig : TypeConfig
    {
        [SerializeField] private TConfig _config;
        
        public TConfig Config => _config;
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

    [Serializable] public abstract class ConfigEntryWithConditionalExecution<TConfig> : ConfigEntry<TConfig> where TConfig : TypeConfig
    {
        [Header("InvokeConditions")] 
        [SerializeField] private bool _hasInvokeConditions;
        [SerializeField, AllowNesting, ShowIf("_hasInvokeConditions")] private ConditionalExecutionEntry _conditionalExecution;
        
        public ConditionalExecutionEntry ConditionalExecution => _conditionalExecution;
        
        public override List<INonScriptableObjectUpdatable> GetAllUpdatables()
        {
            List<INonScriptableObjectUpdatable> updatables = base.GetAllUpdatables();
            
            updatables.AddRange(_conditionalExecution.GetAllUpdatables());

            return updatables;
        }
    }
}