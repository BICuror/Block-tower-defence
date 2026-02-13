using UnityEngine;

namespace WorldGeneration
{
    public sealed class IslandHeightMapToGridConverter
    {
        public BlockGrid Convert(int[,] heightMap, IslandData islandData)
        {
            BlockGrid blockGrid = new BlockGrid(islandData.IslandSize, islandData.IslandMaxHeight);

            for (int x = 0; x < islandData.IslandSize; x++)
            {        
                for (int z = 0; z < islandData.IslandSize; z++)
                {
                    if (heightMap[x, z] > 0) blockGrid.SetBlock(new Vector3Int(x, heightMap[x, z], z));  

                    for (int y = heightMap[x, z] - 1; y >= 0; y--)
                    {
                        blockGrid.SetBlock(new Vector3Int(x, y, z));  
                    }
                }
            }

            return blockGrid;
        }
    }
}