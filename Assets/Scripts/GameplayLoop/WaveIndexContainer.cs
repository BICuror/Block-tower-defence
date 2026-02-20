using UnityEngine;

public sealed class WaveIndexContainer : MonoBehaviour
{
    private static WaveIndexContainer _instance;
    
    public static WaveIndexContainer Instance => _instance;
    
    private int _currentWave;

    private void Awake() => _instance = this;
    
    public int GetCurrentWave() => _currentWave;
    public void IncreaseWaveCounter() => _currentWave++;
}
