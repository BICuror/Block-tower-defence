using EventLinks;
using System;

public sealed class OnDeathEventLink : ParameterlessEventLink
{
    protected override void SubscribeToEvent(Action action)
    {
        Args.Owner.Health.Died += action;
    }

    protected override void UnsubscribeToEvent(Action action)
    {
        Args.Owner.Health.Died -= action;
    }
}