using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWavesData", menuName = "SpawnManagement/EnemyWavesData")]

public class EnemyWavesData : ScriptableObject
{
    [SerializeField] private EnemyWaveGroup[] _waveGroups;
    public EnemyWaveGroup[] WaveGroups => _waveGroups;

    [SerializeField] private float _waveHealth;
    public float WaveHealth => _waveHealth;
}
