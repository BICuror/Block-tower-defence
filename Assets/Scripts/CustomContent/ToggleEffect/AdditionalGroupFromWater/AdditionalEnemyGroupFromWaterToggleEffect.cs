using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;
using Zenject;
using Combat;

public sealed class AdditionalEnemyGroupFromWaterToggleEffect : GlobalToggleEffect
{
    [Inject] private EnemySpawnGroupCompiler _enemySpawnGroupCompiler;
    [Inject] private IslandHeightMapHolder _islandHeightMapHolder;
    [Inject] private IslandDataContainer _islandDataContainer;
    [Inject] private WaveStateMachine _waveStateMachine;
    [Inject] private RoadMapHolder _roadMapHolder;
    private AdditionalEnemyGroupToggleEffectData.AdditionalEnemyGroup _additionalEnemyGroup;

    private List<Vector2Int> _checkDirections = new List<Vector2Int>()
    {
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.right
    };
    
    public void SetAdditionalEnemyGroup(AdditionalEnemyGroupToggleEffectData.AdditionalEnemyGroup additionalEnemyGroup)
    {
        _additionalEnemyGroup = additionalEnemyGroup;
    }
    
    public override void Enable()
    {
        _waveStateMachine.StateStarted += TrySpawn;
    }

    private void TrySpawn(WaveState waveState)
    {
        if (waveState != WaveState.Attack) return;

        float waveHealth = _additionalEnemyGroup.GroupHealth;
        
        List<Vector3> validSpawnPositions = GetLessThanZeroPositions();

        List<EnemyData> enemyDatas = _enemySpawnGroupCompiler.GetEnemyGroupPart(_additionalEnemyGroup.GroupParts, ref waveHealth);

        for (int i = 0; i < enemyDatas.Count && validSpawnPositions.Count > 0; i++)
        {
            int randomPositionIndex = Random.Range(0, validSpawnPositions.Count);

            EnemyFactory.Instance.CreateEnemy(enemyDatas[i], validSpawnPositions[randomPositionIndex]);

            validSpawnPositions.RemoveAt(randomPositionIndex);
        }
    }

    private List<Vector3> GetLessThanZeroPositions()
    {
        List<Vector3> spawnPositions = new List<Vector3>();
        
        float minimalDistanceToCenter = Args.GetArgument<float>("MinimalDistanceToCenter");

        int islandSize = _islandDataContainer.Data.IslandSize;

        int centerIndex = islandSize / 2 - 1;
        
        for (int x = 0; x < islandSize; x++)
        {
            for (int z = 0; z < islandSize; z++)
            {
                if (Vector2.Distance(new Vector2(centerIndex, centerIndex), new Vector2(x, z)) < minimalDistanceToCenter) continue;
                
                if (IsValidSpawnPosition(x, z))
                {
                    spawnPositions.Add(new Vector3(x, -2, z));
                }
            }
        }
        
        return spawnPositions;
    }

    private bool IsValidSpawnPosition(int x, int z)
    {
        if (_islandHeightMapHolder.Map[x, z] <= 0 && _roadMapHolder.Map[x, z] == false)
        {
            for (int i = 0; i < _checkDirections.Count; i++)
            {
                int checkX = x + _checkDirections[i].x;
                int checkZ = z + _checkDirections[i].y;
                
                if (!IsValidPosition(checkX, checkZ)) continue;
                
                if (_roadMapHolder.Map[checkX, checkZ])
                {
                    return true;
                }
            }
        }

        return false;
        
        bool IsValidPosition(int x, int z) => x >= 0 && x < _islandDataContainer.Data.IslandSize && z >= 0 && z < _islandDataContainer.Data.IslandSize;
    }
    
    public override void Disable()
    {
        _waveStateMachine.StateStarted -= TrySpawn;
    }
}