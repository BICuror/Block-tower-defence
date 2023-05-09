using UnityEngine;
using UnityEngine.Events;

public sealed class WaveManager : MonoBehaviour
{
    [SerializeField] private int _maxWave;

    [SerializeField] private bool _isEndless;

    private int _currentWave;

    public UnityEvent WaveStarted;
    public UnityEvent WaveStopped;

    public int GetCurrentWave() => _currentWave;

    private void Start()
    {
        StartWave();
    }

    public void StartWave()
    {
        _currentWave++;
        WaveStarted?.Invoke();
    }

    public void StopWave()
    {
        WaveStarted?.Invoke();
    }
}
