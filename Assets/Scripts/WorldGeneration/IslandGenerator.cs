using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class IslandGenerator : MonoBehaviour
    {
        [SerializeField] private IslandTileTerrainGenerator _islandTileTerrainGenerator;
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private IslandDecorationGenerator _islandDecorationGenerator;
        [Inject] private EnviromentCreator _enviromentCreator;
        [Inject] private BiomeMapGenerator _biomeMapGenerator;
        [Inject] private HeightMapGenerator _heightMapGenerator;
        [Inject] private IslandTerrainMeshCreator _islandTerrainMeshCreator;
        [Inject] private IslandGridHolder _islandGridHolder;
        [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
        
        private IslandData _islandData => _islandDataContainer.Data;

        public void GenerateIsland()
        {
            GenerateNewSeeds();

            GetHeightMap();

            ConvertHeightMapToBlockGrid();

            GenerateTerrainMesh();
                
            GenerateDecorations();

            CreateEnviroment();
        }

        private void GenerateNewSeeds()
        {
            RandomExstentions.ReInitializeUnityRandom();
            
            _heightMapGenerator.GenerateNewSeed();

            _biomeMapGenerator.GenerateNewSeed();
        }

        private void GetHeightMap()
        {
            _islandHeightMapHolder.SetMap(_heightMapGenerator.GenerateHeightMap(_biomeMapGenerator));
        }
        
        private void ConvertHeightMapToBlockGrid()
        {
            IslandHeightMapToGridConverter converter = new();

            _islandGridHolder.SetGrid(converter.Convert(_islandHeightMapHolder.Map, _islandData));
        }

        private void GenerateTerrainMesh()
        {
            _islandTerrainMeshCreator.CreateMesh(_islandGridHolder.Grid);
            
            _islandTileTerrainGenerator.GenerateTerrain();
        }

        private void GenerateDecorations()
        {
            _islandDecorationGenerator.GenerateDecorations(_islandGridHolder.Grid, Vector2.zero); 
        }
     
        private void CreateEnviroment()
        {
            int centerPoint = _islandData.CenterPositionIndex;

            _enviromentCreator.CreateEnviroment(new Vector3(centerPoint, _islandGridHolder.Grid.GetMaxHeight(centerPoint, centerPoint), centerPoint));
        }
    }
}
