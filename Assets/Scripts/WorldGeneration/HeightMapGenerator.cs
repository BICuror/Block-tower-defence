using UnityEngine;
using System.Collections.Generic;
using Zenject;

namespace WorldGeneration
{
    public sealed class HeightMapGenerator
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;

        private BiomeMapGenerator _biomeMapGenerator;

        private Vector2 _heightSeed;

        private int[,] _heightMap;
        public int[,] HeightMap => _heightMap;

        Vector2Int[] _clearingDirections = new Vector2Int[4]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        public void GenerateNewSeed()
        {
            _heightSeed = new Vector2(Random.Range(-100000f, 100000f), Random.Range(-100000f, 100000f));
        }

        public int[,] GenerateHeightMap(BiomeMapGenerator biomeMapGenerator)
        {
            _biomeMapGenerator = biomeMapGenerator;

            float[,] heightMap = new float[_islandData.IslandSize, _islandData.IslandSize];

            for (int x = 0; x < _islandData.IslandSize; x++)
            {
                for (int y = 0; y < _islandData.IslandSize; y++)
                {
                    float height = GetHeight(new Vector2Int(x, y));

                    heightMap[x, y] = height;
                }
            }

            ApplyEdgeReductionMap(heightMap);
            
            if (_islandData.CenterShouldBeFlat) heightMap = GetFlatCenterHeightMap(heightMap);

            if (_islandData.IslandSmoothingType != IslandData.SmoothingType.None) heightMap = SmoothHeightMap(heightMap);

            int[,] roundedHeightMap = TransformHeightMapToInt(heightMap);

            ClearSingleBlocks(roundedHeightMap);

            _heightMap = roundedHeightMap;

            return roundedHeightMap;
        }

        private float GetHeight(Vector2Int position)
        {
            float height = 0f;

            IslandData.Biome biomeAtCurrentPosition = _biomeMapGenerator.GetBiomeAt(position);

            height += GetAverageNoiseAtPoint(position, biomeAtCurrentPosition.Noises);

            height *= biomeAtCurrentPosition.HeightMultiplier;

            return height;
        }
        
        private float GetAverageNoiseAtPoint(Vector2Int position, IslandData.NoiseSetting[] noiseSettings)
        {
            float result = 0;

            for (int i = 0; i < noiseSettings.Length; i++)
            {
                float perlinNoiseX = _heightSeed.x + position.x / (float)_islandData.IslandSize * noiseSettings[i].NoiseScale.x;
                float perlinNoiseY = _heightSeed.y + position.y / (float)_islandData.IslandSize * noiseSettings[i].NoiseScale.y;

                float height = noiseSettings[i].NoiseCurve.Evaluate(Mathf.PerlinNoise(perlinNoiseX, perlinNoiseY));

                result += height;
            }

            result /= noiseSettings.Length;

            return result;
        }

        private void ApplyEdgeReductionMap(float[,] heightMap)
        {
            int half = (int)(_islandData.IslandSize / 2);

            float highBorder = half * _islandData.EdgePrecantageCutout;
            float range = half - highBorder;

            for (int x = 0; x < _islandData.IslandSize; x++)
            {
                for (int z = 0; z < _islandData.IslandSize; z++)
                {
                    float distance = Vector2Int.Distance(new Vector2Int(half, half), new Vector2Int(x, z)) - highBorder;

                    heightMap[x, z] *= _islandData.BorderCurve.Evaluate(1 - distance / range);
                }
            }
        }

        private float[,] SmoothHeightMap(float[,] initialHeightMap)
        {   
            Vector2Int[] directions = GetSmoothingDirections();

            float[,] smoothedHeightMap = new float[_islandData.IslandSize, _islandData.IslandSize];

            for (int x = 1; x < _islandData.IslandSize - 1; x++)
            {
                for (int y = 1; y < _islandData.IslandSize - 1; y++)
                {
                    float averageHeightAround = 0f;

                    for (int i = 0; i < directions.Length; i++)
                    {
                        averageHeightAround += initialHeightMap[x + directions[i].x, y + directions[i].y];
                    }

                    averageHeightAround /= directions.Length;

                    smoothedHeightMap[x, y] = Mathf.Lerp(initialHeightMap[x, y], averageHeightAround, _islandData.SmoothingStrength);
                }
            }

            return smoothedHeightMap;
        }

        private Vector2Int[] GetSmoothingDirections()
        {
            Vector2Int[] directions;

            if (_islandData.IslandSmoothingType == IslandData.SmoothingType.CloseNeibours)
            {
                directions = new Vector2Int[4]
                {
                    new Vector2Int(1, 0),
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, -1)
                };
            }
            else 
            {
                directions = new Vector2Int[8]
                {
                    new Vector2Int(1, 0),
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, -1),
                    new Vector2Int(1, 1),
                    new Vector2Int(-1, -1),
                    new Vector2Int(-1, 1),
                    new Vector2Int(1, -1)
                };
            }

            return directions;
        }

        private int[,] TransformHeightMapToInt(float[,] heightMap)
        {
            int[,] roundedHeightMap = new int[_islandData.IslandSize, _islandData.IslandSize];

            for (int x = 1; x < _islandData.IslandSize - 1; x++)
            {
                for (int y = 1; y < _islandData.IslandSize - 1; y++)
                {
                    roundedHeightMap[x, y] = Mathf.RoundToInt(heightMap[x, y]);
                }
            }

            return roundedHeightMap;
        }

        private void ClearSingleBlocks(int[,] heightMap)
        {
            for (int x = 1; x < _islandData.IslandSize - 1; x++)
            {
                for (int y = 1; y < _islandData.IslandSize - 1; y++)
                {
                    if (_islandData.ClearSingleSolidBlocks) TryClearSolidBlock(heightMap, x, y);

                    if (_islandData.ClearSingleEmptyBlocks) TryClearEmptyBlock(heightMap, x, y);
                }
            }
        }

        private void TryClearEmptyBlock(int[,] heightMap, int x, int y)
        {
            int solidBlocksAround = 0;

            float averageHeightAround = 0;

            for (int i = 0; i < _clearingDirections.Length; i++)
            {
                if (heightMap[x, y] <= heightMap[x + _clearingDirections[i].x, y + _clearingDirections[i].y])
                {
                    solidBlocksAround++; break;
                }
                else
                {
                    averageHeightAround += heightMap[x + _clearingDirections[i].x, y + _clearingDirections[i].y];
                }
            }

            if (solidBlocksAround == 0)
            {
                averageHeightAround /= _clearingDirections.Length;

                heightMap[x, y] = Mathf.RoundToInt(averageHeightAround);
            }
        }

        private void TryClearSolidBlock(int[,] heightMap, int x, int y)
        {
            int emptyBlocksAround = 0;

            float averageHeightAround = 0;

            for (int i = 0; i < _clearingDirections.Length; i++)
            {
                if (heightMap[x, y] >= heightMap[x + _clearingDirections[i].x, y + _clearingDirections[i].y])
                {
                    emptyBlocksAround++; break;
                }
                else
                {
                    averageHeightAround += heightMap[x + _clearingDirections[i].x, y + _clearingDirections[i].y];
                }
            }

            if (emptyBlocksAround == 0)
            {
                averageHeightAround /= _clearingDirections.Length;

                heightMap[x, y] = Mathf.RoundToInt(averageHeightAround);
            }
        }

        private float[,] GetFlatCenterHeightMap(float[,] heightMap)
        {
            List<Vector2Int> indexesOfCenter = new List<Vector2Int>();

            int middleIndex = _islandData.MiddleIndex;

            float heightSumm = 0f;

            for (int x = 0; x < _islandData.IslandSize; x++)
            {
                for (int z = 0; z < _islandData.IslandSize; z++)
                {
                    if (Mathf.Abs(middleIndex - x) + Mathf.Abs(middleIndex - z) <= _islandData.FlatRadius)
                    {
                        heightSumm += heightMap[x, z];

                        indexesOfCenter.Add(new Vector2Int(x, z));
                    }
                } 
            } 

            heightSumm /= indexesOfCenter.Count;

            for (int i = 0; i < indexesOfCenter.Count; i++)
            {
                heightMap[indexesOfCenter[i].x, indexesOfCenter[i].y] = heightSumm + _islandData.FlatHeightIncrease; 
            }

            return heightMap;
        }
    }
}
