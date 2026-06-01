using UnityEngine;
using Zenject;

public sealed class WaveIndexContainer : MonoBehaviour
{
    [Inject] private IslandDataContainer _islandDataContainer;
    
    private static WaveIndexContainer _instance;
    
    public static WaveIndexContainer Instance => _instance;
    
    private int _currentWave;

    private void Awake() => _instance = this;
    
    public int GetCurrentWave() => _currentWave;
    public void IncreaseWaveCounter() => _currentWave++;

    public WaveContent GetCurrentWaveContent()
    {
        return _islandDataContainer.Data.WavesContentConfig.GetWaveContent(GetCurrentWave());
    }
}