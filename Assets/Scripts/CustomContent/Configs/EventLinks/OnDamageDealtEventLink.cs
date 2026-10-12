using EventLinks;
using System;

public sealed class OnDamageDealtEventLink : EventLinkWithParameter<float>
{
    protected override void SubscribeToEvent(Action<float> action)
    {
        Args.Owner.ValueModifierContainer.DamageDealt += action;
    }

    protected override void UnsubscribeToEvent(Action<float> action)
    {
        Args.Owner.ValueModifierContainer.DamageDealt -= action;
    }
}