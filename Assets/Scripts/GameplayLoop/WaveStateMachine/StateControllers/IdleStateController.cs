using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using WorldGeneration;
using UnityEngine;
using System.Linq;
using Navigation;
using Zenject;

public sealed class IdleStateController : WaveStateController
{
    [SerializeField] private CameraPositionController _cameraPositionController;
    [SerializeField] private TerrainAnimator _roadAnimator;
    [Inject] private EnemySpawnGroupCompiler _enemySpawnGroupCompiler;
    [Inject] private IslandDecorationContainer _decorationContainer;
    [Inject] private NavigationMapGenerator _navigationMapGenerator;
    [Inject] private OptionalTaskManager _optionalTaskManager; 
    [Inject] private ItemContainerManager _itemContainerManager;
    [Inject] private EnemyBiomeContainer _enemyBiomesContainer;
    [Inject] private EnemyBiomeGenerator _enemyBiomeGenerator;
    [Inject] private RoadMapGenerator _roadMapGenerator;
    [Inject] private SelectionManager _selectionManager;
    [Inject] private RoadGenerator _roadGenerator;
    [Inject] private ItemFactory _itemFactory;
    [Inject] private WaveIndexContainer _waveIndexContainer;

    public override WaveState GetControlledState() => WaveState.Idle;

    protected override async UniTask OnEnterStateStarted()
    {
        _waveIndexContainer.IncreaseWaveCounter();

        _decorationContainer.ActivateAllDecorations();
        _enemyBiomesContainer.DestroyOldBiomes();

        TryGenerateNewEnemyBiome();

        _enemySpawnGroupCompiler.GenerateWaveSeed();
        
        RegenerateRoads();
        
        RegenerateEnemyBiomes();
        
        _enemyBiomesContainer.EnableBiomesTerrain(TransitionInDuration);
        _roadAnimator.StartAppearing(TransitionInDuration);
    }

    protected override async UniTask OnEnterStateCompleted()
    {
        RandomExstentions.ReInitializeUnityRandom();
        
        await _itemContainerManager.UpdateContainedItems();
        
        _cameraPositionController.SetDefaultPosition();
        
        _selectionManager.TryStartQueuedSelection();
        
        _itemContainerManager.UnlockContainer();
    }

    protected override async UniTask OnQuitStateStarted()
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
        _optionalTaskManager.GenerateTasksAndModifyRoadMap();
        _navigationMapGenerator.GenerateMap();
        _roadGenerator.GenerateRoads();
    }

    [Button("RegenerateRoads")]
    public void RegenerateRoadsButton()
    {
        FindObjectsByType<BloodCollector>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).ToList().ForEach(bloodCollector => Destroy(bloodCollector.gameObject));
        FindObjectsByType<Chest>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).ToList().ForEach(chest => Destroy(chest.gameObject));
        RegenerateRoads();
    }
}