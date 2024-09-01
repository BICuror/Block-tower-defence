using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Navigation;

namespace WorldGeneration
{
    public sealed class RoadGenerator : MonoBehaviour
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;
        
        [Inject] private TextureManager _textureManager;
        [Inject] private HeightMapGenerator _heightMapGenerator;
        [Inject] private RoadMapGenerator _roadMapGenerator;
        [Inject] private IslandDecorationContainer _islandDecorationContainer;
        [SerializeField] private AdditionalNavigationPointsPositionGenerator _additionalNavigationPointsPositionGenerator;
        [SerializeField] private AdditionalNavigationPointsSpawner _additionalNavigationPointsSpawner;
        [Inject] private IslandGridHolder _islandGridHolder;
 
        private BlockGrid _roadGrid;
        public BlockGrid RoadGrid => _roadGrid;

        [SerializeField] private TerrainSetter _roadTerrainSetter;
        [Inject] private RoadMapHolder _roadMapHolder;
        [SerializeField] private EnemyNavigator _enemyNavigator;

        public void GenerateRoads()
        {
            int[,] heightMap = _heightMapGenerator.HeightMap;

            _roadMapGenerator.GenerateRoads();
            List<Vector2Int> chestPositions = _additionalNavigationPointsPositionGenerator.GeneratePositions(_roadMapGenerator.GetEnemyPositions());
            _additionalNavigationPointsSpawner.GeneratePointsPositions(chestPositions, _roadMapGenerator.GetEnemyPositions());
            //_enemyNavigator.GenerateNodeMap();

            bool[,] roadMap = _roadMapHolder.Map;
            _roadGrid = ConvertRoadBlockGrid(roadMap, heightMap);

            GenerateRoadMesh(_roadGrid);
        }

        private void GenerateRoadMesh(BlockGrid roadBlockGrid)
        {
            RoadMeshGenerator roadMeshGenerator = new RoadMeshGenerator();

            roadMeshGenerator.SetupGenerator(roadBlockGrid, _textureManager);
            roadMeshGenerator.SetIslandGrid(_islandGridHolder.Grid);
            
            Mesh roadMesh = roadMeshGenerator.GetMesh();
            
            _roadTerrainSetter.SetMesh(roadMesh); 
        }

        private BlockGrid ConvertRoadBlockGrid(bool[,] roadMap, int[,] heightMap)
        {
            BlockGrid roadGrid = new BlockGrid(_islandData.IslandSize, _islandData.IslandMaxHeight);

            for (int x = 0; x < _islandData.IslandSize; x++)
            {        
                for (int z = 0; z < _islandData.IslandSize; z++)
                {
                    if (roadMap[x, z] == true) 
                    {
                        if (heightMap[x, z] == 0) 
                        {
                            roadGrid.SetBlockType(new Vector3Int(x, 1, z), BlockType.RoadOnWater);  
                        }
                        else
                        {
                            roadGrid.SetBlockType(new Vector3Int(x, heightMap[x, z], z), BlockType.Road);  
                        }
                        

                        _islandDecorationContainer.SetActiveDecorations(x, z, false);
                    }
                }
            }

            return roadGrid;
        }

        private int[,] CreateNewHeaightMap(bool[,] roadMap, int[,] heightMap)
        {
            int[,] roadHeightMap = new int[_islandData.IslandSize, _islandData.IslandSize];

            for (int x = 0; x < _islandData.IslandSize; x++)
            {        
                for (int z = 0; z < _islandData.IslandSize; z++)
                {
                    if (roadMap[x, z])
                    {
                        if (heightMap[x, z] == 0)
                        {
                            roadHeightMap[x, z] = 1;
                        }
                        else
                        {
                            roadHeightMap[x, z] = heightMap[x, z];
                        }
                    }
                }
            }

            return roadHeightMap;
        }
    }
}