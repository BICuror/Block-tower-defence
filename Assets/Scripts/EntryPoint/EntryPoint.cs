using UnityEngine;
using Zenject;
using WorldGeneration;
using Combat;

public sealed class EntryPoint : MonoBehaviour
{
    [SerializeField] private IslandGenerator _islandGenerator;
    [Inject] private WaveManager _waveManager; 

    private void Start()
    {
        _islandGenerator.GenerateIsland();

        _waveManager.PrepeareWave();
    }
}
