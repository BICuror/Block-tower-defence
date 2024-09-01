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
                    if (heightMap[x, z] > 0) blockGrid.SetBlockType(new Vector3Int(x, heightMap[x, z], z), BlockType.Surface);  

                    for (int y = heightMap[x, z] - 1; y >= 0; y--)
                    {
                        blockGrid.SetBlockType(new Vector3Int(x, y, z), BlockType.Rock);  
                    }
                }
            }

            return blockGrid;
        }
    }
}