using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class SpawnerRoadNodeGenerator
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;

        [Inject] private RoadNodeGenerator _roadNodeGenerator;

        public Vector2Int GetRandomEnemySpawnerNodeIndex(List<Vector2Int> exsistingBiomesIndexList)
        {
            IReadOnlyList<int> xNodes = _roadNodeGenerator.XNodes;
            IReadOnlyList<int> zNodes = _roadNodeGenerator.ZNodes;

            List<Vector2Int> possibleNodes = new List<Vector2Int>();

            for (int x = 0; x < xNodes.Count; x++)
            {
                for (int z = 0; z < zNodes.Count; z++)
                {
                    Vector2Int index = new Vector2Int(x, z);
                    
                    if(IsFarEnoughFromExsistingBiomes(index, exsistingBiomesIndexList) && 
                       _islandData.SpawnerPositionValidator.IsValidPosition(x, xNodes.Count, z, zNodes.Count, exsistingBiomesIndexList))
                    {
                        possibleNodes.Add(index);   
                    }
                }
            }
            
            return possibleNodes[Random.Range(0, possibleNodes.Count)];
        }

        private bool IsFarEnoughFromExsistingBiomes(Vector2Int index, List<Vector2Int> exsistingBiomesIndexList)
        {
            return !exsistingBiomesIndexList.Exists(exsistingIndex => Vector2Int.Distance(exsistingIndex, index) < _islandData.MinimalBiomeIndexDistance);
        }
    }
}