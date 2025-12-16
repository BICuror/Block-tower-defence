using WorldGeneration;
using UnityEngine;
using Navigation;
using Zenject;
using Combat;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;

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

        TryGenerateNewEnemyBiome();

        RegenerateRoads();
        
        RegenerateEnemyBiomes();
        
        _enemyBiomesContainer.EnableBiomesTerrain(TransitionInDuration);
        _roadAnimator.StartAppearing(TransitionInDuration);
    }

    protected override void OnEnterStateCompleted()
    {
        RandomExstentions.ReInitializeUnityRandom();
        
        _selectionManager.TryEnqueueNewBuildingSelection();
        _selectionManager.TryStartQueuedSelection();
        _itemContainerManager.UpdateContainedItems().Forget();
        _itemContainerManager.UnlockContainer();
    }

    protected override void OnQuitStateStarted()
    {
        _itemContainerManager.LockContainer();
        _itemFactory.DestroyAllUnusedItems();
    }

    private void TryGenerateNewEnemyBiome()
    {
        RandomExstentions.ReInitializeUnityRandom();
        
        _enemyBiomeGenerator.TryGenerateNewBiome();
    }

    private void RegenerateEnemyBiomes()
    {
        RandomExstentions.ReInitializeUnityRandom();
        
        _enemyBiomesContainer.RegenerateBiomes();
        _enemyBiomesContainer.GenerateBiomesDecorations();
    }
    
    private void RegenerateRoads()
    {
        RandomExstentions.ReInitializeUnityRandom();
        
        _roadMapGenerator.GenerateRoads();
        _optionalTaskGenerator.GenerateTasksAndModifyRoadMap();
        _navigationMapGenerator.GenerateMap();
        _roadGenerator.GenerateRoads();
    }

    [Button("RegenerateRoads")]
    public void RegenerateRoadsButton() => RegenerateRoads();
}