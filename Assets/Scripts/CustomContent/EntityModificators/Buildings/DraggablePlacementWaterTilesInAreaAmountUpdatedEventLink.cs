using ScriptableLogic.ArgumentItem;
using UnityEngine;
using Zenject;
using Configs;
using System;

namespace ScriptableLogic.EventLinks
{
    public sealed class DraggablePlacementTilesInAreaAmountUpdatedEventLink : EventLinkWithParameter<int>
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        [ArgumentItem("AreaScaleValue")] private ValueArgumentItem _areaScaleValue;
        private GameObject _areaDisplay;
        
        public event Action<int> OnTilesAmountUpdated; 

        protected override void SubscribeToEvent(Action<int> action)
        {
            Args.Owner.ComponentsContainer.Get<DraggableObject>().Placed += CalculateTilesAmount;
            
            OnTilesAmountUpdated += action;
        }
    
        protected override void UnsubscribeToEvent(Action<int> action)
        {
            Args.Owner.ComponentsContainer.Get<DraggableObject>().Placed -= CalculateTilesAmount;
            
            OnTilesAmountUpdated -= action;
        }
        
        private void CalculateTilesAmount()
        {
            Vector2Int currentPosition = new Vector2Int(Mathf.RoundToInt(Args.Owner.transform.position.x), Mathf.RoundToInt(Args.Owner.transform.position.z));
            
            int emptyTilesNearby = TileMap.CountValidPositionsInRadius(IsEmptyTile, currentPosition, 0, _areaScaleValue.GetRoundedValue());
            
            OnTilesAmountUpdated?.Invoke(emptyTilesNearby);
        }
    
        private bool IsEmptyTile(Vector2Int position) => !TileMap.HasTile(position, LayerSettingType.AnyTerrain);
    }
}