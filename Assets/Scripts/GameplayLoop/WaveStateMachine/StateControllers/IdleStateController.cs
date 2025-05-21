using WorldGeneration;
using UnityEngine;
using Navigation;
using Zenject;
using Combat;

public sealed class IdleStateController : WaveStateController
{
    [SerializeField] private TerrainAnimator _roadAnimator;
    [Inject] private IslandDecorationContainer _decorationContainer;
    [Inject] private NavigationMapGenerator _navigationMapGenerator;
    [Inject] private OptionalTaskGenerator _optionalTaskGenerator; 
    [Inject] private ItemContainerManager _itemContainerManager;
    [Inject] private EnemyBiomeContainer _enemyBiomesContainer;
    [Inject] private EnemyBiomeGenerator _enemyBiomeGenerator;
    [Inject] private RoadMapGenerator _roadMapGenerator;
    [Inject] private SelectionManager _selectionManager;
    [Inject] private RoadGenerator _roadGenerator;
    [Inject] private ItemFactory _itemFactory;
    [Inject] private WaveManager _waveManager;

    public override WaveState GetControlledState() => WaveState.Idle;

    protected override void OnEnterStateStarted()
    {
        _waveManager.IncreaseWaveCounter();

        _decorationContainer.ActivateAllDecorations();
        _enemyBiomesContainer.DestroyOldBiomes();
        _enemyBiomeGenerator.TryGenerateNewBiome();
        _roadMapGenerator.GenerateRoads();
        _optionalTaskGenerator.GenerateTasksAndModifyRoadMap();
        _navigationMapGenerator.GenerateMap();
        _roadGenerator.GenerateRoads();
        _enemyBiomesContainer.RegenerateBiomes();
        _enemyBiomesContainer.GenerateBiomesDecorations();

        _enemyBiomesContainer.EnableBiomesTerrain(TransitionInDuration);
        _roadAnimator.StartAppearing(TransitionInDuration);
        
        _itemFactory.CreateStartWaveItem(_itemContainerManager.transform.position);
    }

    protected override void OnEnterStateCompleted()
    {
        _itemContainerManager.UpdateContainedItems();
        _selectionManager.TryStartQueuedSelection();
        _itemContainerManager.UnlockContainer();
    }

    protected override void OnQuitStateStarted()
    {
        _itemContainerManager.LockContainer();
        _itemFactory.DestoyAllUnusedItems();
    }
}