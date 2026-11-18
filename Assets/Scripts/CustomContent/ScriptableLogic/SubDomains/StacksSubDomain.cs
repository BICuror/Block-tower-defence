using ScriptableLogic.EventLinks;
using ScriptableLogic.Conditions;
using ScriptableLogic.Commands;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Configs;
using System;

namespace ScriptableLogic.SubDomains
{
    public sealed class StacksSubDomain : SubDomain
    {
        [Argument("MaxStacks")] private int _maxStacks;
        private int _currentStacks;
        
        public int CurrentStacks => _currentStacks; 
        public bool IsMaxStacks => _currentStacks == _maxStacks;
        
        public event Action<int> OnStacksChanged;
        
        public void ChangeStacks(int valueChange)
        {
            _currentStacks += valueChange;
            
            _currentStacks = Mathf.Clamp(_currentStacks, 0, _maxStacks);
    
            OnStacksChanged?.Invoke(_currentStacks);
        }
    
        public void SetStacks(int value)
        {
            _currentStacks = value;
            
            _currentStacks = Mathf.Clamp(_currentStacks, 0, _maxStacks);
    
            OnStacksChanged?.Invoke(_currentStacks);
        }
    }
    
    public sealed class ChangeStacksCommand : SubDomainCommand<StacksSubDomain>, ICommand
    {
        [Argument("StacksChangeValue")] private int _chargeChangeValue;
        
        public UniTask Execute()
        {
            SubDomain.ChangeStacks(_chargeChangeValue);
            
            return UniTask.CompletedTask;
        }
    }
    
    public sealed class SetStacksCommand : SubDomainCommand<StacksSubDomain>, ICommand
    {
        [Argument("StacksSetValue")] private int _stackValue;
        
        public UniTask Execute()
        {
            SubDomain.SetStacks(_stackValue);
            
            return UniTask.CompletedTask;
        }
    }
    
    public sealed class HasEnoughStacksCondition : SubDomainCondition<StacksSubDomain>, ICondition
    {
        [Argument("RequiredStacksAmount")] private int _requiredStacksAmount;
        
        public bool GetValue()
        {
            return SubDomain.CurrentStacks >= _requiredStacksAmount;
        }
    }
    
    public sealed class IsMaxStacks : SubDomainCondition<StacksSubDomain>, ICondition
    {
        public bool GetValue()
        {
            return SubDomain.IsMaxStacks;
        }
    }
    
    public sealed class OnStacksChangedEventLink : EventLinkWithParameter<int>
    {
        protected override void SubscribeToEvent(Action<int> action)
        {
            Args.Domain.GetSubDomain<StacksSubDomain>().OnStacksChanged += action;
        }
    
        protected override void UnsubscribeToEvent(Action<int> action)
        {
            Args.Domain.GetSubDomain<StacksSubDomain>().OnStacksChanged -= action;
        }
    }
}