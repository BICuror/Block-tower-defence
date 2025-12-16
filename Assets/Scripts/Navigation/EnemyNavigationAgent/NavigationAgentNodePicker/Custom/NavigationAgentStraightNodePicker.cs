using System.Collections.Generic;
using UnityEngine;
using Navigation;

public sealed class NavigationAgentStraightNodePicker : NavigationAgentNodePicker
{
    private List<Vector2Int> _checkDirections = new()
    { 
        Vector2Int.up, 
        Vector2Int.down, 
        Vector2Int.right, 
        Vector2Int.left
    };
    

    public override NavigationNode PickNavigationNode(NavigationMap navigationMap, NavigationMapLayer layer, Vector2Int position)
    {
        List<NavigationNode> nearbyNodes = GetNodesAroundPosition(navigationMap, position, _checkDirections);

        return PickNavigationNode(layer, nearbyNodes);
    }
}