using WorldGeneration;
using UnityEngine;


public sealed class EntryPoint : MonoBehaviour
{
    [SerializeField] private IslandGenerator _islandGenerator;
    [SerializeField] private WaveStateMachine _waveManager; 

    private void Start()
    {
        _islandGenerator.GenerateIsland();

        _waveManager.TransitionIntoIdle();
    }
}
