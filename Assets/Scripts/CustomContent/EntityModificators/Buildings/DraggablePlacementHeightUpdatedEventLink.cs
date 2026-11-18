using UnityEngine;
using ScriptableLogic.EventLinks;
using System;

namespace ScriptableLogic.EventLinks
{
    public sealed class DraggablePlacementHeightUpdatedEventLink : EventLinkWithParameter<int>
    {
        private event Action<int> PlacementHeightUpdated; 
        
        protected override void SubscribeToEvent(Action<int> action)
        {
            Args.Owner.ComponentsContainer.Get<DraggableObject>().Placed += InvokeHeightUpdated;
            Args.Owner.ComponentsContainer.Get<DraggableObject>().OnDrag += InvokeHeightUpdated;
            
            PlacementHeightUpdated += action;
        }

        protected override void UnsubscribeToEvent(Action<int> action)
        {
            Args.Owner.ComponentsContainer.Get<DraggableObject>().Placed -= InvokeHeightUpdated;
            Args.Owner.ComponentsContainer.Get<DraggableObject>().OnDrag -= InvokeHeightUpdated;
            
            PlacementHeightUpdated -= action;
        }

        private void InvokeHeightUpdated()
        {
            PlacementHeightUpdated?.Invoke(Mathf.RoundToInt(Args.Owner.transform.position.y) - 2);
        }
    }
}