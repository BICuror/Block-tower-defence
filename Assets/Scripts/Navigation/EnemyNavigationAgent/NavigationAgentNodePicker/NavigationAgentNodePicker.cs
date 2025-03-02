using System.Collections.Generic;
using Navigation;
using UnityEngine;

public abstract class NavigationAgentNodePicker
{
    protected abstract List<Vector2Int> CheckDirections { get; }
    
    public abstract NavigationNode PickNavigationNode(NavigationMap navigationMap, NavigationMapLayer layer, Vector2Int position);
    
    protected List<NavigationNode> GetNodesAroundPosition(NavigationMap navigationMap, Vector2Int position)
    {
        List<NavigationNode> nearbyNodes = new();
            
        CheckDirections.ForEach(direction =>
        {
            if (navigationMap.NodeExists(position + direction))
            {
                nearbyNodes.Add(navigationMap.GetNode(position + direction));
            }
        });

        return nearbyNodes;
    }
}