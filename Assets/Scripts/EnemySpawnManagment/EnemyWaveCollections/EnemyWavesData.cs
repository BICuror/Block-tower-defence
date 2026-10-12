using UnityEngine;

namespace Combat.EnemySpawnSystem
{
    [CreateAssetMenu(fileName = "EnemyWavesData", menuName = "SpawnManagement/EnemyWavesData")]
   
    public class EnemyWavesData : ScriptableObject
    {
        [SerializeField] private EnemyWaveGroupConfig[] _waveGroups;
        public EnemyWaveGroupConfig[] WaveGroups => _waveGroups;
    }
}