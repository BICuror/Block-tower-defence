using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class RoadGenerator : MonoBehaviour
    {
        [Inject] private HeightMapGenerator _heightMapGenerator;
        [Inject] private IslandDecorationContainer _islandDecorationContainer;
        [Inject] private IslandGridHolder _islandGridHolder;
        [Inject] private RoadMapHolder _roadMapHolder;
        [Inject] private IslandDataContainer _islandDataContainer;
        [SerializeField] private RoadTileTerrainGenerator _terrainGenerator;
        [SerializeField] private TerrainSetter _roadBottomTerrainSetter;
        [SerializeField] private TerrainSetter _roadTerrainSetter;
        private BlockGrid _roadGrid;
        
        public BlockGrid RoadGrid => _roadGrid;
        private IslandData _islandData => _islandDataContainer.Data;

        public void GenerateRoads()
        {
            int[,] heightMap = _heightMapGenerator.HeightMap;
            bool[,] roadMap = _roadMapHolder.Map;

            _roadGrid = ConvertRoadBlockGrid(roadMap, heightMap);

            GenerateRoadMesh(_roadGrid);
            _terrainGenerator.GenerateTerrain();
        }

        private void GenerateRoadMesh(BlockGrid roadBlockGrid)
        {
            RoadMeshGenerator roadMeshGenerator = new RoadMeshGenerator();

            roadMeshGenerator.SetupGenerator(roadBlockGrid);
            
            _roadTerrainSetter.SetMesh(roadMeshGenerator.GetDefaultMesh()); 
            
            _roadBottomTerrainSetter.SetMesh(roadMeshGenerator.GetBottomMesh());
        }

        private BlockGrid ConvertRoadBlockGrid(bool[,] roadMap, int[,] heightMap)
        {
            BlockGrid roadGrid = new BlockGrid(_islandData.IslandSize, _islandData.IslandMaxHeight);

            for (int x = 0; x < _islandData.IslandSize; x++)
            {        
                for (int z = 0; z < _islandData.IslandSize; z++)
                {
                    if (roadMap[x, z])
                    {
                        int height = heightMap[x, z];
                        
                        if (height == 0) height = 1;

                        roadGrid.SetBlock(new Vector3Int(x, height, z));
                        
                        _islandDecorationContainer.SetActiveDecorations(x, z, false);
                    }
                }
            }

            return roadGrid;
        }
    }
}