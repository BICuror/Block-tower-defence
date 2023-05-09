using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] private TextureManager _textureManager;

    [SerializeField] private DecorationContainer _islandDecorationContainer;

    [SerializeField] private NavMeshSurface _navMeshSurface;

    [SerializeField] private NavMeshLinksGenerator _navMeshLinksGenerator;

    [SerializeField] private RoadMapGenerator _roadMapGenerator;

    [SerializeField] private NodeGenerator _nodeGenerator;

    [SerializeField] private TerrainSetter _roadTerrainSetter;

    [SerializeField] private EnemyBiomeGenerator _enemyBiomeGenerator;

    public void GenerateRoads(int[,] heightMap)
    {
        bool[,] roadMap = _roadMapGenerator.GenerateRoads(_enemyBiomeGenerator.GetEnemyBiomesPositions(), _nodeGenerator.GetAllNodes());
          
        GenerateRoadMesh(ConvertRoadBlockGrid(roadMap, heightMap));

        GenerateNavMesh(roadMap, heightMap);
    }

    private void GenerateNavMesh(bool[,] roadMap, int[,] heightMap)
    {
        _navMeshSurface.BuildNavMesh();
        
        _navMeshLinksGenerator.DestroyAllStairs();

        _navMeshLinksGenerator.GenrateStairs(heightMap, roadMap);
    }

    private void GenerateRoadMesh(BlockGrid roadBlockGrid)
    {
        RoadMeshGenerator roadMeshGenerator = new RoadMeshGenerator();

        roadMeshGenerator.SetupGenerator(roadBlockGrid, _textureManager);
        
        Mesh roadMesh = roadMeshGenerator.GetMesh();
        
        _roadTerrainSetter.SetMesh(roadMesh); 
    }

    private BlockGrid ConvertRoadBlockGrid(bool[,] roadMap, int[,] heightMap)
    {
        IslandData islandData = IslandDataContainer.GetData();

        BlockGrid roadGrid = new BlockGrid(islandData.IslandSize, islandData.IslandMaxHeight);

        for (int x = 0; x < islandData.IslandSize; x++)
        {        
            for (int z = 0; z < islandData.IslandSize; z++)
            {
                if (roadMap[x, z] == true) 
                {
                    if (heightMap[x, z] == 0) heightMap[x, z] = 1;

                    roadGrid.SetBlockType(new Vector3Int(x, heightMap[x, z], z), BlockType.Road);  

                    _islandDecorationContainer.SetActiveDecorations(x, z, false);
                }
            }
        }

        return roadGrid;
    }
}
