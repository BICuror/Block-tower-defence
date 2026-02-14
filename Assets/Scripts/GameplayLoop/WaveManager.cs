using UnityEngine;

public sealed class WaveManager : MonoBehaviour
{
    private int _currentWave;

    public int GetCurrentWave() => _currentWave;
    public void IncreaseWaveCounter() => _currentWave++;
}
