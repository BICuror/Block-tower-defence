using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class SpawnerRoadNodeGenerator
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;

        [Inject] private RoadNodeGenerator _roadNodeGenerator;

        public Vector2Int GetRandomEnemySpawnerNodeIndex(IReadOnlyList<Vector2Int> exsistingBiomesIndexList)
        {
            IReadOnlyList<int> xNodes = _roadNodeGenerator.XNodes;
            IReadOnlyList<int> zNodes = _roadNodeGenerator.ZNodes;

            List<Vector2Int> possibleNodes = new List<Vector2Int>();

            for (int x = 0; x < xNodes.Count; x++)
            {
                for (int z = 0; z < zNodes.Count; z++)
                {
                    if(_islandData.SpawnerPositionValidator.IsValidPosition(x, xNodes.Count, z, zNodes.Count))
                    {
                        possibleNodes.Add(new Vector2Int(x, z));   
                    }
                }
            }
            
            for (int i = 0; i < exsistingBiomesIndexList.Count; i++)
            {
                for (int j = 0; j < possibleNodes.Count; j++)
                {
                    if (exsistingBiomesIndexList[i] == possibleNodes[j])
                    {
                        possibleNodes.RemoveAt(j);

                        break;
                    }
                }
            }
            
            return possibleNodes[Random.Range(0, possibleNodes.Count)];
        }
    }
}