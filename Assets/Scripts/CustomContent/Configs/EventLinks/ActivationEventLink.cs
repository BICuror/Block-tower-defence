using EventLinks;
using System;

public sealed class ActivationEventLink : ParameterlessEventLink
{
    protected override void SubscribeToEvent(Action action)
    {
        Args.Owner.Activated += action;
    }

    protected override void UnsubscribeToEvent(Action action)
    {
        Args.Owner.Activated -= action;
    }
}