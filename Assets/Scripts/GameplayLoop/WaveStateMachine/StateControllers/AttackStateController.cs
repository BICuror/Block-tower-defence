using Combat.EnemySpawnSystem;
using Cysharp.Threading.Tasks;
using Combat.UpgradeCharges;
using WorldGeneration;
using UnityEngine;
using Zenject;

public sealed class AttackStateController : WaveStateController
{
    [Inject] private EnemySpawnSystemController _enemySpawnSystem;
    [Inject] private EnemyBiomeContainer _enemyBiomesContainer;
    [Inject] private RoadNodeGenerator _roadNodeGenerator;
    [SerializeField] private TerrainAnimator _roadAnimator;

    public override WaveState GetControlledState() => WaveState.Attack;
    
    private void Awake() => _roadNodeGenerator.SetupNodes();
    
    protected override async UniTask OnQuitStateStarted()
    {
        _enemyBiomesContainer.IncreaseBiomesStages();

        _roadAnimator.StartDisappearing(TransitionOutDuration);
        _enemyBiomesContainer.DisableBiomesTerrain(TransitionOutDuration);
    }

    protected override async UniTask OnEnterStateCompleted()
    {
        _enemySpawnSystem.StartWave().Forget();
    }
}