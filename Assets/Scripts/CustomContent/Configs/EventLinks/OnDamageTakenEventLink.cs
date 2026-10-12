using EventLinks;
using System;

public sealed class OnDamageTakenEventLink : EventLinkWithParameter<float>
{
    protected override void SubscribeToEvent(Action<float> action)
    {
        Args.Owner.Health.DamageReceived += action;
    }

    protected override void UnsubscribeToEvent(Action<float> action)
    {
        Args.Owner.Health.DamageReceived -= action;
    }
}