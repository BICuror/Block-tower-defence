using System.Collections.Generic;
using UnityEngine;
using Navigation;

public sealed class NavigationNodePickerDiagonalPicker : NavigationAgentNodePicker
{
    private const int CHANCE_TO_GO_DIRECTIONALY = 50;
    
    private List<Vector2Int> _checkDirections = new()
    { 
        Vector2Int.up, 
        Vector2Int.down, 
        Vector2Int.right, 
        Vector2Int.left,
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1)
    };
    
    private readonly List<Vector2Int> _straightCheckDirections = new()
    { 
        Vector2Int.up, 
        Vector2Int.down, 
        Vector2Int.right, 
        Vector2Int.left,
    };
    
    private readonly List<Vector2Int> _cornerCheckDirections = new()
    { 
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1)
    };
    
    public override NavigationNode PickNavigationNode(NavigationMap navigationMap, NavigationMapLayer layer, Vector2Int position)
    {
        List<NavigationNode> nearbyNodes = GetNodesAroundPosition(navigationMap, position, _checkDirections);
        NavigationNode bestStraightNode = PickStraightNavigationNode(navigationMap, layer, position);
        
        NavigationNode bestNearbyNode = null;
        int bestNearbyFutureNodeWeight = layer.GetNodeWeight(bestStraightNode);
        
        nearbyNodes.ForEach(node =>
        {
            if (layer.GetNodeWeight(node) > 0)
            {
                List<NavigationNode> futureNodes = GetNodesAroundPosition(navigationMap, node.RoundedPosition, _cornerCheckDirections);
    
                if (futureNodes.Count > 0)
                {
                    NavigationNode bestFutureNode = PickNavigationNode(layer, futureNodes); 
                
                    int futureNodeWeight = layer.GetNodeWeight(bestFutureNode);
                
                    if (futureNodeWeight > 0 && 
                        ((PickType == WeightPickType.Minimal && futureNodeWeight < bestNearbyFutureNodeWeight - 2) ||
                         (PickType == WeightPickType.Maximal && futureNodeWeight > bestNearbyFutureNodeWeight + 1)))
                    {
                        bestNearbyFutureNodeWeight = futureNodeWeight;
                        bestNearbyNode = node;
                    }
                }
            }
        });
        
        if (bestNearbyNode != null) return bestNearbyNode;
        
        NavigationNode bestDiagonalNode = PickNavigationNode(layer, nearbyNodes);
            
        if (layer.GetNodeWeight(bestDiagonalNode) > 0) return bestDiagonalNode;
        
        return bestStraightNode;
    }

    private NavigationNode PickStraightNavigationNode(NavigationMap navigationMap, NavigationMapLayer layer, Vector2Int position)
    {
        List<NavigationNode> nearbyNodes = GetNodesAroundPosition(navigationMap, position, _straightCheckDirections);

        return PickNavigationNode(layer, nearbyNodes);
    }
}