using Cysharp.Threading.Tasks;
using UnityEngine;
using EventLinks;
using Configs;
using System;

public sealed class StacksSubDomain : SubDomain
{
    [Argument("MaxStacks")] private int _maxStacks;
    private int _currentStacks;
    
    public int CurrentStacks => _currentStacks; 
    
    public event Action<int> OnStacksChanged;
    
    public void ChangeStacks(int valueChange)
    {
        _currentStacks += valueChange;
        
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

public sealed class HasEnoughStacksCondition : SubDomainCondition<StacksSubDomain>, ICondition
{
    [Argument("RequiredStacksAmount")] private int _requiredStacksAmount;
    
    public bool GetValue()
    {
        return SubDomain.CurrentStacks >= _requiredStacksAmount;
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