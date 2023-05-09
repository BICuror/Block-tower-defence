using UnityEngine;

public sealed class BiomeMapGenerator
{
    private Vector2 _biomeSeed;
    
    public void GenerateNewBiomeSeed()
    {
        _biomeSeed = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }

    public IslandData.Biome GetBiomeAt(Vector2Int position)
    {
        IslandData islandData = IslandDataContainer.GetData();

        float noisesAverageValue = GetAverageBiomeNoiseValue(position);

        float biomeAppearanceChanse = 0f;

        for (int i = 0; i < islandData.Biomes.Length; i++)
        {
            biomeAppearanceChanse += islandData.Biomes[i].AppearRate;

            if (biomeAppearanceChanse >= noisesAverageValue) 
            {
                return islandData.Biomes[i];
            }
        }

        UnityEngine.Debug.Log("There is no biome at x: " + position.x.ToString() + " z: " + position.y.ToString());
        return new IslandData.Biome();
    }

    private float GetAverageBiomeNoiseValue(Vector2Int position)
    {
        IslandData islandData = IslandDataContainer.GetData();

        float average = 0f;

        for (int i = 0; i < islandData.BiomeGenerationNoises.Length; i++)
        {
            float perlinNoiseX = _biomeSeed.x + position.x / (float)islandData.IslandSize * islandData.BiomeGenerationNoises[i].NoiseScale.x;
            float perlinNoiseY = _biomeSeed.y + position.y / (float)islandData.IslandSize * islandData.BiomeGenerationNoises[i].NoiseScale.y;

            float evaluatedValue = islandData.BiomeGenerationNoises[i].NoiseCurve.Evaluate(Mathf.PerlinNoise(perlinNoiseX, perlinNoiseY));

            average += evaluatedValue;
        }

        average /= islandData.BiomeGenerationNoises.Length;

        return average;
    }
}
