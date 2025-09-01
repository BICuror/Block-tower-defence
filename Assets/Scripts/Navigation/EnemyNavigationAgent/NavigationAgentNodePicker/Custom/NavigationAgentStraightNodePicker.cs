using System.Collections.Generic;
using UnityEngine;
using Navigation;
using System.Linq;
using System;

using Random = UnityEngine.Random;

public sealed class NavigationAgentStraightNodePicker : NavigationAgentNodePicker
{
    protected override List<Vector2Int> CheckDirections => new List<Vector2Int>
    { 
        Vector2Int.up, 
        Vector2Int.down, 
        Vector2Int.right, 
        Vector2Int.left
    };
    

    public override NavigationNode PickNavigationNode(NavigationMap navigationMap, NavigationMapLayer layer, Vector2Int position)
    {
        List<NavigationNode> nearbyNodes = GetNodesAroundPosition(navigationMap, position);

        return PickNavigationNode(layer, nearbyNodes);
    }
}