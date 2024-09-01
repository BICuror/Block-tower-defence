using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class EnemyBiomeMapToGridConverter : MonoBehaviour
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;
        
        [Inject] RoadMapHolder _roadMapHolder;
        [Inject] IslandGridHolder _islandGridHolder;

        public BlockGrid ConvertEnemyMapToGrid(bool[,] enemyBiomeMap, int biomeStage, Vector2Int biomePosition)
        {
            int radius = _islandData.EnemyBiomeStages[biomeStage].EnemyBiomeRadius;

            BlockGrid enemyBiomeGrid = new BlockGrid(radius * 2 + 1, _islandData.IslandMaxHeight);

            bool[,] roadMap = _roadMapHolder.Map; 

            for (int x = 0; x < radius * 2 + 1; x++)
            {        
                for (int z = 0; z < radius * 2 + 1; z++)
                {   
                    if (enemyBiomeMap[x, z])
                    {   
                        int height = 0;

                        Vector2Int currentPosition = biomePosition + new Vector2Int(x, z);

                        if (IsInBound(currentPosition))
                        {
                            if (roadMap[currentPosition.x, currentPosition.y] == true) continue;

                            height = _islandGridHolder.Grid.GetMaxHeight(currentPosition.x, currentPosition.y);
                        }

                        BlockType blockType = BlockType.Corruption;

                        if (height == 0) 
                        {
                            height = _islandData.CorruptionLessZeroHeight;

                            blockType = BlockType.CorruptionOnWater;
                        }
                        
                        for (int y = height; y >= 0; y--)
                        {
                            enemyBiomeGrid.SetBlockType(new Vector3Int(x, y, z), blockType);  
                        }    
                    }
                }
            }

            return enemyBiomeGrid;
        }

        private bool IsInBound(Vector2Int position)
        {
            return (position.x < _islandData.IslandSize && position.y < _islandData.IslandSize && position.x  >= 0 && position.y >= 0);      
        }
    }
}