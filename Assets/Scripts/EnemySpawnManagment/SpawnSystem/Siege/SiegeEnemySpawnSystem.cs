using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using WorldGeneration;
using UnityEngine;
using Zenject;

namespace Combat.EnemySpawnSystem.Siege
{
    public sealed class SiegeEnemySpawnSystem : MonoBehaviour, IEnemySpawnSystem
    {
        [Inject] private EnemySpawnGroupContainer _enemySpawnGroupContainer;
        [Inject] private GlobalEnemyContainer _globalEnemyContainer;
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private WaveIndexContainer _waveIndexContainer;
        [Inject] private IslandHeightMapHolder _islandHeightMap;
        private List<SiegePillar> _siegePillars = new();
        private bool _allEnemiesSpawned;
        
        [Header("Spawn")]
        [SerializeField] private EnemyEntity _pillarEntity;
        [SerializeField] private int _pillarSpawnRangeFrom;

        [Header("CompleteSequence")] 
        [SerializeField] private float _enemyKillDelay = 0.05f;
        
        public bool AllEnemiesSpawned => _allEnemiesSpawned;

        public void PrepareWave(WaveContent waveContent, List<EnemySpawner> spawners)
        {
            _siegePillars = SpawnSiegePillars(waveContent);
        }

        public async UniTask SpawnWave(WaveContent waveContent, List<EnemySpawner> spawners)
        {
            CancellationTokenSource cancellationTokenSource = new();
            
            _allEnemiesSpawned = false;
            
            foreach (EnemySpawner enemySpawner in spawners)
            {
                SpawnEnemiesFromSpawner(enemySpawner, cancellationTokenSource.Token).Forget();
            }

            await AwaitPillarsDeath();
            
            _allEnemiesSpawned = true;

            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();

            IReadOnlyList<EnemyEntity> allEnemies = new List<EnemyEntity>(_globalEnemyContainer.Entities);
            
            foreach (EnemyEntity enemyEntity in allEnemies)
            {
                enemyEntity.Health.Die();

                await UniTask.WaitForSeconds(_enemyKillDelay);
            }
        }

        private async UniTask SpawnEnemiesFromSpawner(EnemySpawner spawner, CancellationToken cancellationToken)
        {
            List<EnemySpawnData> enemySpawnData = _enemySpawnGroupContainer.GetEnemySpawnDataForSpawner(spawner);

            while (!AllEnemiesSpawned)
            {
                await spawner.SpawnEnemyGroup(enemySpawnData, cancellationToken);
            }
        }
        
        private async UniTask AwaitPillarsDeath()
        {
            for (int i = 0; i < _siegePillars.Count; i++)
            {
                _siegePillars[i].EnablePillar();

                await UniTask.WaitWhile(() => _siegePillars[i]);
            }
        }

        private List<SiegePillar> SpawnSiegePillars(WaveContent waveContent)
        {
            List<SiegePillar> siegePillars = new();
            
            int pillarsAmount = waveContent.PillarsAmount;
            float pillarHealth = waveContent.PillarsHealth;

            float angleStep = 360f / pillarsAmount;
            float offset = Random.Range(0, 360f);
            
            for (int i = 0; i < pillarsAmount; i++)
            {
                Vector2Int spawnPosition = TileMap.FindClosestValidPosition(IsValidPosition, GetOffset(i).ToIntVector(), 0, 10);

                EnemyEntity pillar = Instantiate(_pillarEntity, spawnPosition.ToVector3Int(_islandHeightMap.GetHeightSafe(spawnPosition) + 1), Quaternion.identity);

                pillar.StatContainer.Get<MaxHealth>().SetDefault(pillarHealth);
                
                _globalEnemyContainer.Add(pillar);
                siegePillars.Add(pillar.GetComponent<SiegePillar>());
            }
            
            return siegePillars;

            Vector2 GetOffset(int index)
            {
                float xOffset = Mathf.Cos(Mathf.Deg2Rad * (angleStep * index + offset)) * _pillarSpawnRangeFrom + _islandDataContainer.Data.IslandRadius;
                float zOffset = Mathf.Sin(Mathf.Deg2Rad * (angleStep * index + offset)) * _pillarSpawnRangeFrom + _islandDataContainer.Data.IslandRadius;

                return new Vector3(xOffset, zOffset);
            }

            bool IsValidPosition(Vector2Int position) => TileMap.HasTile(position, LayerSettingType.SolidTerrain) && !TileMap.HasTile(position, LayerSettingType.SolidObjects) && !TileMap.HasTile(position, LayerSettingType.RoadTerrain);
        }
    }
}