using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWavesData", menuName = "SpawnManagement/EnemyWavesData")]

public class EnemyWavesData : ScriptableObject
{
    [SerializeField] private EnemyWaveGroup[] _waveGroups;
    public EnemyWaveGroup[] WaveGroups => _waveGroups;
}