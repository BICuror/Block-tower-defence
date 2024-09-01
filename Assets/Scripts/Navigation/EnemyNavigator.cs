using UnityEngine;
using Zenject;
using WorldGeneration;
using System.Collections.Generic;
using Navigation;

public sealed class EnemyNavigator : MonoBehaviour
{
    [Inject] private RoadMapHolder _roadMapHolder;
    [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
    [Inject] private RoadMapGenerator _roadMapGenerator;
    [Inject] private IslandHeightMapHolder _heightMapHolder;
    [SerializeField] private GameObject Chest;
    public static EnemyNavigator Instance; 

    [Inject] private NavigationMapHolder _navigationMap;

    private void Awake() => Instance = this;

    public void GenerateNodeMap()
    {
    
    }

    public NavigationNode GetNavigationNode(Vector3 position)
    {
        Vector2Int nodePosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));

        if (_navigationMap.Map.GetNode(nodePosition.x, nodePosition.y) == null) Debug.LogError("No node found");
        return _navigationMap.Map.GetNode(nodePosition.x, nodePosition.y);
    }
}
