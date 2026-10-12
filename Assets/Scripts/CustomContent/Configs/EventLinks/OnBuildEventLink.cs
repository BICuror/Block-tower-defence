using EventLinks;
using System;

public sealed class OnBuildEventLink : ParameterlessEventLink
{
    protected override void SubscribeToEvent(Action action)
    {
        Args.Owner.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted += action;
    }

    protected override void UnsubscribeToEvent(Action action)
    {
        Args.Owner.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted -= action;
    }
}