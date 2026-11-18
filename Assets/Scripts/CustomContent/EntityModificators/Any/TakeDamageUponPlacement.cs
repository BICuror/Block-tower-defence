using ScriptableLogic.EventLinks;
using System;

namespace ScriptableLogic.EventLinks
{
    public sealed class DraggablePlacementEventLink : ParameterlessEventLink
    {
        protected override void SubscribeToEvent(Action action)
        {
            Args.Owner.ComponentsContainer.Get<DraggableObject>().Placed += action;
        }

        protected override void UnsubscribeToEvent(Action action)
        {
            Args.Owner.ComponentsContainer.Get<DraggableObject>().Placed -= action;
        }
    }
    
    public sealed class DraggablePickedUpEventLink : ParameterlessEventLink
    {
        protected override void SubscribeToEvent(Action action)
        {
            Args.Owner.ComponentsContainer.Get<DraggableObject>().PickedUp += action;
        }

        protected override void UnsubscribeToEvent(Action action)
        {
            Args.Owner.ComponentsContainer.Get<DraggableObject>().PickedUp -= action;
        }
    }
    
    public sealed class BuildingBuiltEventLink : ParameterlessEventLink
    {
        protected override void SubscribeToEvent(Action action)
        {
            Args.Owner.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted += action;
        }

        protected override void UnsubscribeToEvent(Action action)
        {
            Args.Owner.ComponentsContainer.Get<BuildingDraggable>().BuildCompleted  -= action;
        }
    }
}