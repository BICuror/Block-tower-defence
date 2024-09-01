using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class OverlappingIslandDecorationsDisabler : MonoBehaviour
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;
        
        [Inject] IslandDecorationContainer _islandDecorationContainer;

        public void DisableOverlappingIslandDecorations(bool[,] enemyBiomeMap, int biomeStage, Vector2Int biomePosition)
        {
            int radius = _islandData.EnemyBiomeStages[biomeStage].EnemyBiomeRadius;

            for (int x = 0; x < radius * 2 + 1; x++)
            {        
                for (int z = 0; z < radius * 2 + 1; z++)
                {   
                    if (enemyBiomeMap[x, z])
                    {
                        _islandDecorationContainer.SetActiveDecorationsIfInBound(biomePosition.x + x, biomePosition.y + z, false);
                    }
                } 
            }
        }
    }
}