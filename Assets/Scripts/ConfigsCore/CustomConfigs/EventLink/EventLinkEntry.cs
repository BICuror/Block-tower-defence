using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using EventLinks;
using Configs;
using System;

namespace Coinfigs
{
    [Serializable] public sealed class EventLinkEntry : ArgumentDataEntryContainer
    {
        [SerializeField] private EventLinkConfig _eventLinkConfig;
        [SerializeField] private List<CommandEntry> _commandEntries;
        
        [Header("InvokeConditions")] 
        [SerializeField] private bool _hasInvokeConditions;
        [SerializeField, AllowNesting, ShowIf("_hasInvokeConditions")] private ConditionalExecutionEntry _conditionalExecution;
        
        public IReadOnlyList<CommandEntry> CommandEntries => _commandEntries;
        public EventLinkConfig Config => _eventLinkConfig;
        public ConditionalExecutionEntry ConditionalExecution => _conditionalExecution;

        protected override bool DisplayArguments => _eventLinkConfig;
        protected override Type GetCurrentTargetType() => _eventLinkConfig.GetContainedType();
        
        public override List<INonScriptableObjectUpdatable> GetAllUpdatables()
        {
            List<INonScriptableObjectUpdatable> updatables = base.GetAllUpdatables();
            
            foreach (CommandEntry commandEntry in _commandEntries)
            {
                updatables.AddRange(commandEntry.GetAllUpdatables());
            }
            
            updatables.AddRange(_conditionalExecution.GetAllUpdatables());
            
            return updatables;
        }

        public override void Update()
        {
            base.Update();
            
            if (_eventLinkConfig) Name = _eventLinkConfig.name;
            else Name = "No config";
            
            if (!Config) return;
            Type eventLinkType = Config.GetContainedType();
            
            if (eventLinkType == null) return;

            bool isGenericEvent = _eventLinkConfig.TryGetGenericType(out List<Type> argumentTypes);
            
            foreach (CommandEntry commandEntry in _commandEntries)
            {
                if (!commandEntry.Config) continue;
                
                Type commandType = commandEntry.GetContainedType();

                if (typeof(INonExecutableCommand).IsAssignableFrom(commandType))
                {
                    commandEntry.ResetConfig();
                    
                    Debug.LogError($"Command type {commandType} is non executable and cannot be used in event statements");
                    continue;
                }
                
                bool isParameterLessCommand = typeof(ICommand).IsAssignableFrom(commandType);
                
                if (isParameterLessCommand) continue;
                
                if (!isGenericEvent) 
                {
                    commandEntry.ResetConfig();
                    
                    Debug.LogError($"Invalid command type of {commandType}, while event is parameterless");
                    continue;
                }
                
                Type genericCommandType = commandType.GetInterfaces()[0].GetGenericArguments()[0];

                if (!argumentTypes.Contains(genericCommandType))
                {
                    Debug.LogError($"Invalid command type of {genericCommandType}, while event only supports {argumentTypes[0]}");
                    
                    commandEntry.ResetConfig();
                }
            }
        }
    }
}

