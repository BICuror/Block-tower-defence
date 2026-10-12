using EventLinks;
using System;

public sealed class OnActionCycledEventLink : ParameterlessEventLink
{
    protected override void SubscribeToEvent(Action action)
    {
        Args.Owner.ComponentsContainer.Get<TaskCycle>().TaskCycled += action;
    }

    protected override void UnsubscribeToEvent(Action action)
    {
        Args.Owner.ComponentsContainer.Get<TaskCycle>().TaskCycled -= action;
    }
}