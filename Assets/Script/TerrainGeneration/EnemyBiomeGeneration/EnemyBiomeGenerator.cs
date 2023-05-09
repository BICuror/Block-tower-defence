using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBiomeGenerator : MonoBehaviour
{
    [SerializeField] private NodeGenerator _nodeGenerator;

    [SerializeField] private TextureManager _textureManager;
    
    [SerializeField] private GameObject _enemyBiome;

    [SerializeField] private DecorationContainer _islandDecorationContainer;

    private BlockGrid _terrainBlockGrid;

    private List<EnemyBiome> _enemyBiomes = new List<EnemyBiome>();

    public void Setup(BlockGrid islandBlockGrid)
    {
        _terrainBlockGrid = islandBlockGrid;

        _nodeGenerator.SetupNodes();
    }

    public void DevelopBiomes()
    {
        for (int i = 0; i < _enemyBiomes.Count; i++)
        {
            _enemyBiomes[i].IncreaseCurrentStage();

            if (_enemyBiomes[i].GetStage() >= IslandDataContainer.GetData().EnemyBiomeStages.Length)
            {
                Destroy(_enemyBiomes[i].gameObject);

                _enemyBiomes.RemoveAt(i);
            }

            _enemyBiomes[i].GenerateBiome();
        }
    }

    public void DeactivateCardDecorations()
    {
        
    }

    public void GenerateNewBiome()
    {
        Vector2Int enemySpawnerNode = _nodeGenerator.GetEnemySpawnerNodes(GetEnemyBiomesPositions());

        GameObject biome = Instantiate(_enemyBiome, Vector3.zero, Quaternion.identity);

        biome.GetComponent<EnemyBiome>().SetCenterPosition(enemySpawnerNode);    

        biome.GetComponent<EnemyBiome>().Setup(_terrainBlockGrid, _textureManager, _islandDecorationContainer);    
        
        biome.GetComponent<EnemyBiome>().GenerateBiome();    

        _enemyBiomes.Add(biome.GetComponent<EnemyBiome>());
    }

    public List<Vector2Int> GetEnemyBiomesPositions()
    {
        List<Vector2Int> result = new List<Vector2Int>();

        for (int i = 0; i < _enemyBiomes.Count; i++)
        {
            result.Add(_enemyBiomes[i].GetCenterPosition());
        }
        
        return result;
    }
}
