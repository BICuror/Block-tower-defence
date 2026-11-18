using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Combat.ValueModifiers;
using Configs;
using System;

namespace ScriptableLogic.Commands.ValueModifiers
{
    public sealed class AddHealValueModifierCommand : AddValueModificatorCommand
    {
        [Argument("IsHealReceiverModifier")] private bool _isHealReceiverModifier;
        
        protected override ValueModifierContainer GetValueModifierContainer()
        {
            if (!_isHealReceiverModifier) return Args.Owner.ValueModifierContainer.HealDealerContainer;
            return Args.Owner.ValueModifierContainer.HealReceiverContainer;
        }
    }
    
    public sealed class AddDamageValueModifierCommand : AddValueModificatorCommand
    {
        [Argument("IsDamageReceiverModifier")] private bool _isDamageReceiverModifier;
        
        protected override ValueModifierContainer GetValueModifierContainer()
        {
            if (!_isDamageReceiverModifier) return Args.Owner.ValueModifierContainer.DamageDealerContainer;
            return Args.Owner.ValueModifierContainer.DamageReceiverContainer;
        }
    }
    
    public abstract class AddValueModificatorCommand : Command, INonExecutableCommand
    {
        [Argument("ValueModifierConfig")] private ValueModifierConfig _valueModifierConfig;
        
        private List<ValueModifier> _valueModifiers = new();
        
        public override UniTask Enable()
        {
            ValueModifierContainer valueModifierContainer = GetValueModifierContainer();
            
            foreach (ValueModifierConfigEntryContainer valueModifierConfigEntry in _valueModifierConfig.ValueModifierConfigEntries)
            {
                ValueModifier valueModifier = (ValueModifier)Activator.CreateInstance(valueModifierConfigEntry.GetContainedType());
                
                valueModifier.InjectArgumentEntries(valueModifierConfigEntry.Arguments);
                
                _valueModifiers.Add(valueModifier);
                
                valueModifierContainer.Add(valueModifier);
            }
            
            return UniTask.CompletedTask;
        }
        
    
        public override UniTask Disable()
        {
            ValueModifierContainer valueModifierContainer = GetValueModifierContainer();
            
            foreach (ValueModifier valueModifier in _valueModifiers)
            {
                valueModifierContainer.Remove(valueModifier);
            }
            
            return UniTask.CompletedTask;
        }
        
        protected abstract ValueModifierContainer GetValueModifierContainer();
    }
}