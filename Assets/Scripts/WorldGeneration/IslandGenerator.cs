using NaughtyAttributes;
using UnityEngine;
using Zenject;
using System;

namespace WorldGeneration 
{
    public sealed class IslandGenerator : MonoBehaviour
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private IslandDecorationGenerator _islandDecorationGenerator;
        [Inject] private EnviromentCreator _enviromentCreator;
        [Inject] private BiomeMapGenerator _biomeMapGenerator;
        [Inject] private HeightMapGenerator _heightMapGenerator;
        [Inject] private IslandTerrainMeshCreator _islandTerrainMeshCreator;
        [Inject] private IslandGridHolder _islandGridHolder;
        [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
        
        private IslandData _islandData => _islandDataContainer.Data;

        [Button] 
        public void GenerateIsland()
        {
            GenerateNewSeedsAndHeightMap();
            ConvertHeightMapToBlockGrid();
            GenerateTerrainMesh();
            GenerateDecorations();
            CreateEnviroment();
        }

        private void GenerateNewSeedsAndHeightMap()
        {
            for (int i = 0; i < 1000; i++)
            {
                RandomExstentions.ReInitializeUnityRandom();
                
                _heightMapGenerator.GenerateNewSeed();
    
                _biomeMapGenerator.GenerateNewSeed();

                if (_heightMapGenerator.TryGenerateHeightMap(_biomeMapGenerator, out int[,] heightMap))
                {
                    _islandHeightMapHolder.SetMap(heightMap);
                    
                    return;
                }
            }
            
            throw new Exception("Failed to generate heightmap due to min and max solid tiles setting");
        }
        
        private void ConvertHeightMapToBlockGrid()
        {
            IslandHeightMapToGridConverter converter = new();

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
            int centerPoint = _islandData.CenterPositionIndex;

            _enviromentCreator.CreateEnviroment(new Vector3(centerPoint, _islandGridHolder.Grid.GetMaxHeight(centerPoint, centerPoint), centerPoint));
        }
    }
}
