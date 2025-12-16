using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Navigation;
using System;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public abstract class NavigationAgentNodePicker
{
    private PickBestNodeWeightDelegate _pickBestNodeWeightDelegate;
    private WeightPickType _currentWieghtPickType;
    
    protected delegate int PickBestNodeWeightDelegate(NavigationMapLayer layer, List<NavigationNode> validNavigationNodes);
    
    public WeightPickType PickType => _currentWieghtPickType;
    
    public void SetWeightPickLogic(WeightPickType weightPickType)
    {
        _currentWieghtPickType = weightPickType;
        
        switch (weightPickType)
        {
            case WeightPickType.Minimal: _pickBestNodeWeightDelegate = PickMinimalNodeWeight; break;
            case WeightPickType.Maximal: _pickBestNodeWeightDelegate = PickMaximalNodeWeight; break;
        }
    }
    
    public abstract NavigationNode PickNavigationNode(NavigationMap navigationMap, NavigationMapLayer layer, Vector2Int position);

    protected List<NavigationNode> GetNodesAroundPosition(NavigationMap navigationMap, Vector2Int position, List<Vector2Int> directions)
    {
        List<NavigationNode> nearbyNodes = new();
            
        directions.ForEach(direction =>
        {
            if (navigationMap.NodeExists(position + direction))
            {
                nearbyNodes.Add(navigationMap.GetNode(position + direction));
            }
        });

        return nearbyNodes;
    }

    protected NavigationNode PickNavigationNode(NavigationMapLayer layer, List<NavigationNode> validNavigationNodes)
    {
        int bestWeight = _pickBestNodeWeightDelegate(layer, validNavigationNodes);

        List<NavigationNode> bestNodes = validNavigationNodes.FindAll(node => layer.GetNodeWeight(node) == bestWeight).ToList();
        
        return bestNodes[Random.Range(0, bestNodes.Count)];
    }

    private int PickMinimalNodeWeight(NavigationMapLayer layer, List<NavigationNode> validNavigationNodes)
    {
        int minimalWeight = int.MaxValue;
        
        validNavigationNodes.ForEach(node => minimalWeight = Math.Min(layer.GetNodeWeight(node), minimalWeight));

        return minimalWeight;
    }
    
    private int PickMaximalNodeWeight(NavigationMapLayer layer, List<NavigationNode> validNavigationNodes)
    {
        int maximalWeight = int.MinValue;
        
        validNavigationNodes.ForEach(node => maximalWeight = Math.Max(layer.GetNodeWeight(node), maximalWeight));

        return maximalWeight;
    }

    public enum WeightPickType
    {
        Minimal,
        Maximal
    }
}