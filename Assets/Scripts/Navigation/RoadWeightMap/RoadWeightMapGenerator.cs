using WorldGeneration;
using UnityEngine;
using Zenject;

public sealed class RoadWeightMapGenerator : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private RoadWeightMapHolder _roadWeightMapHolder;
    [Inject] private RoadMapHolder _roadMapHolder;
    
    private static readonly Vector2Int[] _checkDirections = new Vector2Int[4]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left, 
        Vector2Int.right
    };

    public void GenerateRoadWeightMap()
    {
        int[,] weightMap = CreateRoadWeightMap();
        
        _roadWeightMapHolder.SetWeightMap(weightMap);
    }

    private int[,] CreateRoadWeightMap()
    {
        int islandSize = _islandDataContainer.Data.IslandSize;
        int islandRadius = _islandDataContainer.Data.IslandRadius;
        
        int[,] weightMap = new int[islandSize, islandSize];
        
        MapWeights(new Vector2Int(islandRadius, islandRadius), 0);

        return weightMap;
        
        void MapWeights(Vector2Int position, int weight)
        {
            weight++;
            weightMap[position.x, position.y] = weight;

            for (int i = 0; i < _checkDirections.Length; i++)
            {
                Vector2Int checkPosition = _checkDirections[i] + position;
                
                if (!TileMap.IsAValidRoadPosition(checkPosition, _roadMapHolder.Map)) continue;
                
                if (_roadMapHolder.Map[checkPosition.x, checkPosition.y])
                {
                    if (weightMap[checkPosition.x, checkPosition.y] == 0 || weightMap[checkPosition.x, checkPosition.y] > weight + 1)
                    {
                        MapWeights(checkPosition, weight);
                    }   
                }
            }
        }
    }
}