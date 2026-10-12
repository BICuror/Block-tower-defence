using Cysharp.Threading.Tasks;
using EventLinks;
using Configs;
using System;

public sealed class ChargeSubDomain : SubDomain
{
    [Argument("MaxCharges")] private int _maxCharges;
    [Argument("DefaultCharges")] private int _defaultCharges;
    private int _currentCharges;
    
    public int CurrentCharges => _currentCharges; 
    
    public event Action<float> OnChargePercentChanged;
    
    protected override void OnInitialize()
    {
        SetDefaultChargesAmount();
    }
    
    public void ChangeCharges(int valueChange)
    {
        _currentCharges += valueChange;
        
        if (_currentCharges > _maxCharges) _currentCharges = _maxCharges;
        else if (_currentCharges < 0) _currentCharges = 0;

        InvokeOnChargePercentChanged();
    }

    public void SetDefaultChargesAmount()
    {
        _currentCharges = _defaultCharges;

        InvokeOnChargePercentChanged();
    }

    private void InvokeOnChargePercentChanged()
    {
        OnChargePercentChanged?.Invoke(_currentCharges / (float)_maxCharges);
    }
}

public sealed class ChangeChargesCommand : SubDomainCommand<ChargeSubDomain>, ICommand
{
    [Argument("ChargeChangeValue")] private int _chargeChangeValue;
    
    public UniTask Execute()
    {
        SubDomain.ChangeCharges(_chargeChangeValue);
        
        return UniTask.CompletedTask;
    }
}

public sealed class SetDefaultAmountOfChargesCommand : SubDomainCommand<ChargeSubDomain>, ICommand
{
    public UniTask Execute()
    {
        SubDomain.SetDefaultChargesAmount();
        
        return UniTask.CompletedTask;
    }
}

public sealed class HasEnoughChargesCondition : SubDomainCondition<ChargeSubDomain>, ICondition
{
    [Argument("RequiredChargesAmount")] private int _requiredChargesAmount;
    
    public bool GetValue()
    {
        return SubDomain.CurrentCharges >= _requiredChargesAmount;
    }
}

public sealed class OnChargePercentChangedEventLink : EventLinkWithParameter<float>
{
    protected override void SubscribeToEvent(Action<float> action)
    {
        Args.Domain.GetSubDomain<ChargeSubDomain>().OnChargePercentChanged += action;
    }

    protected override void UnsubscribeToEvent(Action<float> action)
    {
        Args.Domain.GetSubDomain<ChargeSubDomain>().OnChargePercentChanged -= action;
    }
}