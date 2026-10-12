using EventLinks;
using System;

public sealed class OnActionEventLink : ParameterlessEventLink
{
    protected override void SubscribeToEvent(Action action)
    {
        Args.Owner.ComponentsContainer.Get<TaskCycle>().TaskPerformed += action;
    }

    protected override void UnsubscribeToEvent(Action action)
    {
        Args.Owner.ComponentsContainer.Get<TaskCycle>().TaskPerformed -= action;
    }
}