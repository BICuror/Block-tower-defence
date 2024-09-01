using UnityEngine;
using Zenject;

namespace WorldGeneration
{
    public sealed class BiomeMapGenerator
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;

        private Vector2 _biomeSeed;
        
        public void GenerateNewSeed()
        {
            _biomeSeed = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }

        public IslandData.Biome GetBiomeAt(Vector2Int position)
        {
            float noisesAverageValue = GetAverageBiomeNoiseValue(position);

            float biomeAppearanceChanse = 0f;

            for (int i = 0; i < _islandData.Biomes.Length; i++)
            {
                biomeAppearanceChanse += _islandData.Biomes[i].AppearRate;

                if (biomeAppearanceChanse >= noisesAverageValue) 
                {
                    return _islandData.Biomes[i];
                }
            }

            return _islandData.Biomes[_islandData.Biomes.Length - 1];
        }

        private float GetAverageBiomeNoiseValue(Vector2Int position)
        {
            float average = 0f;

            for (int i = 0; i < _islandData.BiomeGenerationNoises.Length; i++)
            {
                float perlinNoiseX = _biomeSeed.x + position.x / (float)_islandData.IslandSize * _islandData.BiomeGenerationNoises[i].NoiseScale.x;
                float perlinNoiseY = _biomeSeed.y + position.y / (float)_islandData.IslandSize * _islandData.BiomeGenerationNoises[i].NoiseScale.y;

                float evaluatedValue = _islandData.BiomeGenerationNoises[i].NoiseCurve.Evaluate(Mathf.PerlinNoise(perlinNoiseX, perlinNoiseY));

                average += evaluatedValue;
            }

            average /= _islandData.BiomeGenerationNoises.Length;

            return average;
        }
    }
}
