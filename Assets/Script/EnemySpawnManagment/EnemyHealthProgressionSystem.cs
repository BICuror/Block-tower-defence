using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthProgressionSystem : MonoBehaviour
{
    [SerializeField] private WaveManager _waveManager;

    public void MultiplyEnemyHealth(GameObject enemy)
    {
        enemy.GetComponent<EnemyHealth>().MultiplyHealth(_waveManager.GetCurrentWave());
    }
}
