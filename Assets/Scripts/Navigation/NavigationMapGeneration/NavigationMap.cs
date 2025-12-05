using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Navigation
{
    public sealed class NavigationMap
    {
        private readonly List<NavigationMapLayer> _navigationLayers = new();
        private Dictionary<Vector2Int, NavigationNode> _nodeMap = new();
        
        public void SetNode(Vector2Int position, NavigationNode node) => _nodeMap.Add(position, node);
        public bool NodeExists(Vector2Int position) => _nodeMap.ContainsKey(position);
        public NavigationNode GetNode(Vector2Int position) => _nodeMap[position];
        public void ResetNodeMap() => _nodeMap.Clear();
        
        public void ClearAllLayers() => _navigationLayers.Clear();

        public NavigationMapLayer CreateLayerAndAdd(Dictionary<Vector2Int, int> _nodeWeights, NavigationMapLayerType layerType)
        {
            NavigationMapLayer layer = new(_nodeWeights, layerType);
            
            _navigationLayers.Add(layer);
            
            return layer;
        }
        
        public bool HasActiveLayerOfType(NavigationMapLayerType layerType) => _navigationLayers.Exists(layer => layer.LayerType == layerType && layer.IsEnabled);

        public NavigationMapLayer GetLayer(Vector2Int position, NavigationMapLayerType type)
        {
            List<NavigationMapLayer> layers = _navigationLayers.FindAll(layer => layer.LayerType == type && layer.IsEnabled).ToList();
            
            NavigationMapLayer mostSuitableLayer = layers[0];

            layers.ForEach(layer =>
            {
                if (layer.GetNodeWeight(position) < mostSuitableLayer.GetNodeWeight(position))
                {
                    mostSuitableLayer = layer;
                }
            });

            return mostSuitableLayer;
        }
    }

    public sealed class NavigationMapLayer
    {
        public NavigationMapLayer(Dictionary<Vector2Int, int> nodeWeights, NavigationMapLayerType layerType)
        {
            _nodeWeights = nodeWeights;
            _layerType = layerType;
        }
        
        private readonly Dictionary<Vector2Int, int> _nodeWeights;
        private readonly NavigationMapLayerType _layerType;
        private INavigationCondition _navigationCondition;
        
        public NavigationMapLayerType LayerType => _layerType;
        public bool IsEnabled => _navigationCondition == null || _navigationCondition.GetValue();
        
        public void SetNavigationCondition(INavigationCondition condition) => _navigationCondition = condition;
        public int GetNodeWeight(Vector2Int nodePosition) => _nodeWeights[nodePosition];
        public int GetNodeWeight(NavigationNode node) => _nodeWeights[node.RoundedPosition];
        public void SetNodeWeight(Vector2Int nodePosition, int weight) => _nodeWeights[nodePosition] = weight;
    }

    public enum NavigationMapLayerType 
    { 
        Main, 
        AdditionalTask
    }
}