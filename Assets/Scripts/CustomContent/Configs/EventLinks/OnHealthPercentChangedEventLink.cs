using EventLinks;
using System;

public sealed class OnHealthPercentChangedEventLink : EventLinkWithParameter<float>
{
    private event Action<float> OnHealthPercentChanged; 
    
    protected override void SubscribeToEvent(Action<float> action)
    {
        Args.Owner.Health.Healed += InvokeHealthPercentChangedEvent;
        Args.Owner.Health.Damaged += InvokeHealthPercentChangedEvent;
            
        OnHealthPercentChanged += action;
    }

    protected override void UnsubscribeToEvent(Action<float> action)
    {
        Args.Owner.Health.Healed -= InvokeHealthPercentChangedEvent;
        Args.Owner.Health.Damaged -= InvokeHealthPercentChangedEvent;
            
        OnHealthPercentChanged -= action;
    }

    private void InvokeHealthPercentChangedEvent()
    {
        OnHealthPercentChanged?.Invoke(Args.Owner.Health.GetHpPercent());
    }
}