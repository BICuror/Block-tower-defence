using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Combat.EnemySpawnSystem
{
    public interface IEnemySpawnSystem
    {
        public bool AllEnemiesSpawned { get; }
        public void PrepareWave(WaveContent waveContent, List<EnemySpawner> spawners);
        public UniTask SpawnWave(WaveContent waveContent, List<EnemySpawner> spawners);
    }
}