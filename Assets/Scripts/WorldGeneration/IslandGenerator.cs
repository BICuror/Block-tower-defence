using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class IslandGenerator : MonoBehaviour
    {
        #region Dependencies
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;
        
        [Inject] private IslandDecorationGenerator _islandDecorationGenerator;
        [Inject] private EnviromentCreator _enviromentCreator;
        [Inject] private BiomeMapGenerator _biomeMapGenerator;
        [Inject] private HeightMapGenerator _heightMapGenerator;
        [Inject] private IslandTerrainMeshCreator _islandTerrainMeshCreator;
        [Inject] private IslandGridHolder _islandGridHolder;
        [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
        #endregion

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
            _heightMapGenerator.GenerateNewSeed();

            _biomeMapGenerator.GenerateNewSeed();
        }

        private void GetHeightMap()
        {
            _islandHeightMapHolder.SetMap(_heightMapGenerator.GenerateHeightMap(_biomeMapGenerator));
        }
        
        private void ConvertHeightMapToBlockGrid()
        {
            IslandHeightMapToGridConverter converter = new IslandHeightMapToGridConverter();

            _islandGridHolder.SetGrid(converter.Convert(_islandHeightMapHolder.Map, _islandData));
        }

        private void GenerateTerrainMesh()
        {
            _islandTerrainMeshCreator.CreateMesh(_islandGridHolder.Grid);
        }

        private void GenerateDecorations()
        {
            _islandDecorationGenerator.GenerateDecorations(_islandGridHolder.Grid, Vector2.zero); 
        }
     
        private void CreateEnviroment()
        {
            int _centerPoint = Mathf.RoundToInt(_islandData.IslandSize / 2);

            _enviromentCreator.CreateEnviroment(new Vector3(_centerPoint, _islandGridHolder.Grid.GetMaxHeight(_centerPoint, _centerPoint), _centerPoint));
        }
    }
}
