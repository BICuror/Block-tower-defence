using WorldGeneration;
using UnityEngine;
using Combat;
using Zenject;

public sealed class AttackStateController : WaveStateController
{
    [SerializeField] private TerrainAnimator _roadAnimator;
    [Inject] private RoadNodeGenerator _roadNodeGenerator;
    [Inject] private EnemySpawnSystem _enemySpawnSystem;
    [Inject] private EnemyBiomeContainer _enemyBiomesContainer;

    public override WaveState GetControlledState() => WaveState.Attack;
    
    private void Awake() => _roadNodeGenerator.SetupNodes();
    protected override void OnQuitStateStarted()
    {
        _enemyBiomesContainer.IncreaseBiomesStages();

        _roadAnimator.StartDisappearing(TransitionOutDuration);
        _enemyBiomesContainer.DisableBiomesTerrain(TransitionOutDuration);
    }

    protected override void OnEnterStateCompleted()
    {
        _enemySpawnSystem.StartWave();
    }
}